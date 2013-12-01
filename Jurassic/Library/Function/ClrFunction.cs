﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents a JavaScript function implemented by one or more .NET methods.
    /// </summary>
    public class ClrFunction : FunctionInstance
    {
        bool bindThis;
        private FunctionBinder callBinder;
        private FunctionBinder constructBinder;


        //     INITIALIZATION
        //_________________________________________________________________________________________

                /// <summary>
        /// Creates a new instance of a built-in constructor function.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="name"> The name of the function. </param>
        /// <param name="instancePrototype">  </param>
        protected ClrFunction(ObjectInstance prototype, string name, ObjectInstance instancePrototype)
            : base(prototype)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (instancePrototype == null)
                throw new ArgumentNullException("instancePrototype");

            // This is a constructor so ignore the "this" parameter when the function is called.
            bindThis = true;

            // Search through every method in this type looking for [JSFunction] attributes.
            var callBinderMethods = new List<FunctionBinderMethod>(1);
            var constructBinderMethods = new List<FunctionBinderMethod>(1);
            var methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                // Search for the [JSCallFunction] and [JSConstructorFunction] attributes.
                var callAttribute = (JSCallFunctionAttribute) Attribute.GetCustomAttribute(method, typeof(JSCallFunctionAttribute));
                var constructorAttribute = (JSConstructorFunctionAttribute)Attribute.GetCustomAttribute(method, typeof(JSConstructorFunctionAttribute));

                // Can't declare both attributes.
                if (callAttribute != null && constructorAttribute != null)
                    throw new InvalidOperationException("Methods cannot be marked with both [JSCallFunction] and [JSConstructorFunction].");

                if (callAttribute != null)
                {
                    // Method is marked with [JSCallFunction]
                    callBinderMethods.Add(new FunctionBinderMethod(method, callAttribute.Flags));
                }
                else if (constructorAttribute != null)
                {
                    var binderMethod = new FunctionBinderMethod(method, constructorAttribute.Flags);
                    constructBinderMethods.Add(binderMethod);
                    
                    // Constructors must return ObjectInstance or a derived type.
                    if (typeof(ObjectInstance).IsAssignableFrom(binderMethod.Method.ReturnType) == false)
                        throw new InvalidOperationException(string.Format("Constructors must return {0} (or a derived type).", typeof(ObjectInstance).Name));
                }
            }

            // Initialize the Call function.
            if (callBinderMethods.Count > 0)
                this.callBinder = new FunctionBinder(callBinderMethods);
            else
                this.callBinder = new FunctionBinder(new FunctionBinderMethod(new Func<object>(() => Undefined.Value).Method));

            // Initialize the Construct function.
            if (constructBinderMethods.Count > 0)
                this.constructBinder = new FunctionBinder(constructBinderMethods);
            else
                this.constructBinder = new FunctionBinder(new FunctionBinderMethod(new Func<ObjectInstance>(() => GlobalObject.Object.Construct()).Method));

            // Add function properties.
            this.FastSetProperty("name", name);
            this.FastSetProperty("length", callBinderMethods.FirstOrDefault() == null ? 0 : callBinderMethods.Max(bm => bm.ParameterCount));
            this.FastSetProperty("prototype", instancePrototype);
            instancePrototype.FastSetProperty("constructor", this, PropertyAttributes.NonEnumerable);
        }

        /// <summary>
        /// Creates a new instance of a function which calls the given delegate.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="delegateToCall"> The delegate to call. </param>
        /// <param name="name"> The name of the function.  Pass <c>null</c> to use the name of the
        /// delegate for the function name. </param>
        /// <param name="length"> The "typical" number of arguments expected by the function.  Pass
        /// <c>-1</c> to use the number of arguments expected by the delegate. </param>
        public ClrFunction(ObjectInstance prototype, Delegate delegateToCall, string name = null, int length = -1)
            : base(prototype)
        {
            // Initialize the [[Call]] method.
            var binderMethod = new FunctionBinderMethod(delegateToCall.Method);
            this.callBinder = new FunctionBinder(new FunctionBinderMethod(delegateToCall.Method));

            // Add function properties.
            this.FastSetProperty("name", name != null ? name : binderMethod.Name);
            this.FastSetProperty("length", length >= 0 ? length : binderMethod.ParameterCount);
            this.FastSetProperty("prototype", GlobalObject.Object.Construct());
            this.InstancePrototype.FastSetProperty("constructor", this, PropertyAttributes.NonEnumerable);
        }

        /// <summary>
        /// Creates a new instance of a function which calls one or more provided methods.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="methods"> An enumerable collection of methods that logically comprise a
        /// single method group. </param>
        /// <param name="name"> The name of the function.  Pass <c>null</c> to use the name of the
        /// provided methods for the function name (in this case all the provided methods must have
        /// the same name). </param>
        /// <param name="length"> The "typical" number of arguments expected by the function.  Pass
        /// <c>-1</c> to use the maximum of arguments expected by any of the provided methods. </param>
        public ClrFunction(ObjectInstance prototype, IEnumerable<FunctionBinderMethod> methods, string name = null, int length = -1)
            : base(prototype)
        {
            this.callBinder = new FunctionBinder(methods);

            // Determine the name automatically if it wasn't provided.
            if (name == null)
            {
                // Verify all the methods have the same name.
                name = methods.First().Name;
                if (methods.All(bm => bm.Name == name) == false)
                    throw new ArgumentException("To determine the function name automatically, all of the supplied methods need to have the same name.", "methods");
            }

            // Add function properties.
            this.FastSetProperty("name", name);
            this.FastSetProperty("length", length >= 0 ? length : methods.Max(bm => bm.ParameterCount));
            this.FastSetProperty("prototype", GlobalObject.Object.Construct());
            this.InstancePrototype.FastSetProperty("constructor", this, PropertyAttributes.NonEnumerable);
        }

        

        //     OVERRIDES
        //_________________________________________________________________________________________

        /// <summary>
        /// Calls this function, passing in the given "this" value and zero or more arguments.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword within the function. </param>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The value that was returned from the function. </returns>
        public override object CallLateBound(object thisObject, params object[] arguments)
        {
            return this.callBinder.Call(bindThis == true ? this : thisObject, arguments);
        }

        /// <summary>
        /// Creates an object, using this function as the constructor.
        /// </summary>
        /// <param name="argumentValues"> An array of argument values. </param>
        /// <returns> The object that was created. </returns>
        public override ObjectInstance ConstructLateBound(params object[] argumentValues)
        {
            if (this.constructBinder == null)
                return GlobalObject.Object.Construct();
            return (ObjectInstance)this.constructBinder.Call(this, argumentValues);
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        public override string ToString()
        {
            return string.Format("function {0}() {{ [native code] }}", this.Name);
        }

        ///// <summary>
        ///// Creates a delegate that does type conversion and calls the method represented by this
        ///// object.
        ///// </summary>
        ///// <param name="argumentTypes"> The types of the arguments that will be passed to the delegate. </param>
        ///// <returns> A delegate that does type conversion and calls the method represented by this
        ///// object. </returns>
        //internal BinderDelegate CreateBinder<T>()
        //{
        //    // Delegate types have an Invoke method containing the relevant parameters.
        //    MethodInfo adapterInvokeMethod = typeof(T).GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
        //    if (adapterInvokeMethod == null)
        //        throw new ArgumentException("The type parameter T must be delegate type.", "T");

        //    // Get the argument types.
        //    Type[] argumentTypes = adapterInvokeMethod.GetParameters().Select(p => p.ParameterType).ToArray();

        //    // Create the binder.
        //    return this.callBinder.CreateBinder(argumentTypes);
        //}


    }
}