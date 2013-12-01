﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jurassic.Library
{
    /// <summary>
    /// Provides functionality common to all JavaScript objects.
    /// </summary>
    public class ObjectInstance
    {
        // Internal prototype chain.
        private ObjectInstance prototype;

        // Stores the property names and attributes for this object.
        private HiddenClassSchema schema = HiddenClassSchema.Empty;

        // Stores the property values for this object.
        private object[] propertyValues = new object[4];

        [Flags]
        private enum ObjectFlags
        {
            /// <summary>
            /// Indicates whether properties can be added to this object.
            /// </summary>
            Extensible = 1,
        }

        // Stores flags related to this object.
        private ObjectFlags flags = ObjectFlags.Extensible;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates an Object with no prototype.
        /// </summary>
        private ObjectInstance()
        {
        }

        /// <summary>
        /// Called by derived classes to create a new object instance.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        protected ObjectInstance(ObjectInstance prototype)
        {
            if (prototype == null)
                throw new ArgumentNullException("prototype");
            this.prototype = prototype;
        }

        /// <summary>
        /// Creates an Object with no prototype to serve as the base prototype of all objects.
        /// </summary>
        /// <returns> An Object with no prototype. </returns>
        internal static ObjectInstance CreateRootObject()
        {
            return new ObjectInstance();
        }

        /// <summary>
        /// Creates an Object instance (use ObjectConstructor.Construct rather than this).
        /// </summary>
        /// <returns> An Object instance. </returns>
        internal static ObjectInstance CreateRawObject(ObjectInstance prototype)
        {
            return new ObjectInstance(prototype);
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected virtual string InternalClassName
        {
            get { return "Object"; }
        }

        /// <summary>
        /// Gets the next object in the prototype chain.  There is no corresponding property in
        /// javascript (it is is *not* the same as the prototype property), instead use
        /// Object.getPrototypeOf().
        /// </summary>
        public ObjectInstance Prototype
        {
            get { return this.prototype; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the object can have new properties added
        /// to it.
        /// </summary>
        internal bool IsExtensible
        {
            get { return (this.flags & ObjectFlags.Extensible) != 0; }
            set
            {
                if (value == true)
                    throw new InvalidOperationException("Once an object has been made non-extensible it cannot be made extensible again.");
                this.flags &= ~ObjectFlags.Extensible;
            }
        }

        /// <summary>
        /// Gets or sets the value of a named property.
        /// </summary>
        /// <param name="propertyName"> The name of the property to get or set. </param>
        /// <returns> The property value, or <c>null</c> if the property doesn't exist. </returns>
        public object this[string propertyName]
        {
            get { return GetPropertyValue(propertyName); }
            set { SetPropertyValue(propertyName, value, false); }
        }

        /// <summary>
        /// Gets or sets the value of an array-indexed property.
        /// </summary>
        /// <param name="index"> The index of the property to retrieve. </param>
        /// <returns> The property value, or <c>null</c> if the property doesn't exist. </returns>
        public object this[uint index]
        {
            get { return GetPropertyValue(index); }
            set { SetPropertyValue(index, value, false); }
        }

        /// <summary>
        /// Gets an enumerable list of every property name and value associated with this object.
        /// </summary>
        public virtual IEnumerable<PropertyNameAndValue> Properties
        {
            get
            {
                // Enumerate named properties.
                return this.schema.EnumeratePropertyNamesAndValues(this.InlinePropertyValues);
            }
        }



        //     INLINE CACHING
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets an object to use as a cache key.  Adding, deleting, or modifying properties will
        /// cause the value of this property to change.
        /// </summary>
        public object InlineCacheKey
        {
            get { return this.schema; }
        }

        /// <summary>
        /// Gets the values stored against this object, one for each property.  The index to use
        /// can be retrieved from <see cref="InlineGetPropertyValue"/>,
        /// <see cref="InlineSetPropertyValue"/> or <see cref="InlineSetPropertyValueIfExists"/>.
        /// </summary>
        public object[] InlinePropertyValues
        {
            get { return this.propertyValues; }
        }

        /// <summary>
        /// Gets the value of the given property plus the information needed to speed up access to
        /// the property in future.
        /// </summary>
        /// <param name="name"> The name of the property. </param>
        /// <param name="cachedIndex"> Set to a zero-based index that can be used to get or set the
        /// property value in future (provided the cache key doesn't change). </param>
        /// <param name="cacheKey"> Set to a value that can be compared with the CacheKey property
        /// to determine if the cached index needs to be refreshed.  Can be set to <c>null</c> to
        /// prohibit caching. </param>
        /// <returns> The value of the property, or <c>null</c> if the property doesn't exist. </returns>
        public object InlineGetPropertyValue(string name, out int cachedIndex, out object cacheKey)
        {
            //cachedIndex = this.schema.GetPropertyIndex(name);
            //if (cachedIndex < 0)
            //{
            //    // The property is in the prototype or is non-existant.
            //    cacheKey = null;
            //    if (this.Prototype == null)
            //        return null;
            //    return this.Prototype.Get(name);
            //}
            //cacheKey = this.InlineCacheKey;
            //return this.InlinePropertyValues[cachedIndex];

            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the value of the given property plus retrieves the information needed to speed up
        /// access to the property in future.  If a property with the given name does not exist, or
        /// exists in the prototype chain (and is not a setter) then a new property is created.
        /// </summary>
        /// <param name="name"> The name of the property to set. </param>
        /// <param name="value"> The desired value of the property. </param>
        /// <param name="cachedIndex"> Set to a zero-based index that can be used to get or set the
        /// property value in future (provided the cache key doesn't change). </param>
        /// <param name="cacheKey"> Set to a value that can be compared with the CacheKey property
        /// to determine if the cached index needs to be refreshed.  Can be set to <c>null</c> to
        /// prohibit caching. </param>
        public void InlineSetPropertyValue(string name, object value, out int cachedIndex, out object cacheKey)
        {
            //cachedIndex = this.schema.GetPropertyIndex(name);
            //if (cachedIndex < 0)
            //{
            //    // The property is in the prototype or doesn't exist - add a new property.
            //    this.schema = this.schema.AddProperty(name);
            //    cachedIndex = this.schema.GetPropertyIndex(name);
            //    if (cachedIndex >= this.InlinePropertyValues.Length)
            //    {
            //        // Resize the value array.
            //        Array.Resize(ref this.propertyValues, this.InlinePropertyValues.Length * 2);
            //    }
            //}
            //cacheKey = this.InlineCacheKey;
            //this.InlinePropertyValues[cachedIndex] = value;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the value of the given property plus retrieves the information needed to speed up
        /// access to the property in future.  If a property with the given name exists, but only
        /// in the prototype chain, then a new property is created (unless the property is a
        /// setter, in which case the setter is called and no property is created).  If the
        /// property does not exist at all, then no property is created and the method returns
        /// <c>false</c>.
        /// </summary>
        /// <param name="name"> The name of the property to set. </param>
        /// <param name="value"> The desired value of the property.  This must be a javascript
        /// primitive (double, string, etc) or a class derived from <see cref="ObjectInstance"/>. </param>
        /// <param name="cachedIndex"> Set to a zero-based index that can be used to get or set the
        /// property value in future (provided the cache key doesn't change). </param>
        /// <param name="cacheKey"> Set to a value that can be compared with the CacheKey property
        /// to determine if the cached index needs to be refreshed.  Can be set to <c>null</c> to
        /// prohibit caching. </param>
        /// <returns> <c>true</c> if the property value exists; <c>false</c> otherwise. </returns>
        public bool InlineSetPropertyValueIfExists(string name, object value, out int index, out object cacheKey)
        {
            //index = this.schema.GetPropertyIndex(name);
            //if (index < 0)
            //{
            //    // The property is in the prototype or is non-existant.
            //    cacheKey = null;
            //    if (this.Prototype == null)
            //        return false;
            //    if (this.Prototype.HasProperty(name) == true)
            //    {
            //        // TODO: fix this.
            //        this.Prototype[name] = value;
            //        return true;
            //    }
            //    return false;
            //}
            //cacheKey = this.InlineCacheKey;
            //this.InlinePropertyValues[index] = value;
            //return true;

            throw new NotImplementedException();
        }



        //     PROPERTY MANAGEMENT
        //_________________________________________________________________________________________

        ///// <summary>
        ///// Gets the property descriptor for the property with the given name.  The prototype
        ///// chain is not searched.
        ///// </summary>
        ///// <param name="propertyName"> The name of the property. </param>
        ///// <returns> A property descriptor containing the property value and attributes.  The
        ///// result will be <c>PropertyDescriptor.Undefined</c> if the property doesn't exist. </returns>
        //internal virtual PropertyDescriptor GetOwnProperty(string propertyName)
        //{
        //    PropertyAttributes attributes;
        //    int index = this.schema.GetPropertyIndexAndAttributes(propertyName, out attributes);
        //    if (index == -1)
        //        return PropertyDescriptor.Undefined;
        //    return new PropertyDescriptor(this.propertyValues[index], attributes);
        //}

        ///// <summary>
        ///// Gets the property descriptor for the property with the given array index.  The
        ///// prototype chain is not searched.
        ///// </summary>
        ///// <param name="index"> The array index of the property. </param>
        ///// <returns> A property descriptor containing the property value and attributes.  The
        ///// result will be <c>PropertyDescriptor.Undefined</c> if the property doesn't exist. </returns>
        //internal virtual PropertyDescriptor GetOwnProperty(uint index)
        //{
        //    return GetOwnProperty(index.ToString());
        //}

        ///// <summary>
        ///// Gets the property descriptor for the property with the given name.
        ///// </summary>
        ///// <param name="propertyName"> The name of the property. </param>
        ///// <returns> A property descriptor containing the property value and attributes.  The
        ///// value will be <c>PropertyDescriptor.Undefined</c> if the property doesn't exist. </returns>
        //internal PropertyDescriptor GetProperty(string propertyName)
        //{

        //}

        /// <summary>
        /// Determines if a property with the given name exists.
        /// </summary>
        /// <param name="propertyName"> The name of the property to check. </param>
        /// <returns> <c>true</c> if the property exists on this object or in the prototype chain;
        /// <c>false</c> otherwise. </returns>
        public bool HasProperty(string propertyName)
        {
            return this.GetPropertyValue(propertyName) != null;
        }

        /// <summary>
        /// Gets the value of the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property. </param>
        /// <returns> The value of the property, or <c>null</c> if the property doesn't exist. </returns>
        /// <remarks> The prototype chain is searched if the property does not exist directly on
        /// this object. </remarks>
        public object GetPropertyValue(uint index)
        {
            // Get the descriptor for the property.
            var property = this.GetOwnPropertyDescriptor(index);
            if (property.Exists == true)
            {
                // The property was found!  Call the getter if there is one.
                object value = property.Value;
                var accessor = value as PropertyAccessorValue;
                if (accessor != null)
                    return accessor.GetValue(this);
                return value;
            }

            // The property might exist in the prototype.
            if (this.prototype == null)
                return null;
            return this.prototype.GetPropertyValue(index);
        }

        /// <summary>
        /// Gets the value of the property with the given name.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <returns> The value of the property, or <c>null</c> if the property doesn't exist. </returns>
        /// <remarks> The prototype chain is searched if the property does not exist directly on
        /// this object. </remarks>
        public object GetPropertyValue(string propertyName)
        {
            // Check if the property is an indexed property.
            uint arrayIndex = ArrayInstance.ParseArrayIndex(propertyName);
            if (arrayIndex != uint.MaxValue)
                return GetPropertyValue(arrayIndex);

            ObjectInstance prototypeObject = this;
            do
            {
                // Retrieve information about the property.
                var property = prototypeObject.schema.GetPropertyIndexAndAttributes(propertyName);
                if (property.Exists == true)
                {
                    // The property was found!
                    object value = prototypeObject.propertyValues[property.Index];
                    if ((property.Attributes & (PropertyAttributes.IsAccessorProperty | PropertyAttributes.IsLengthProperty)) == 0)
                        return value;

                    // Call the getter if there is one.
                    if (property.IsAccessor == true)
                        return ((PropertyAccessorValue)value).GetValue(this);

                    // Otherwise, the property is the "magic" length property.
                    return (int)((ArrayInstance)prototypeObject).Length;
                }

                // Traverse the prototype chain.
                prototypeObject = prototypeObject.prototype;
            } while (prototypeObject != null);

            // The property doesn't exist.
            return null;
        }

        /// <summary>
        /// Gets a descriptor for the property with the given array index.
        /// </summary>
        /// <param name="propertyName"> The array index of the property. </param>
        /// <returns> A property descriptor containing the property value and attributes. </returns>
        /// <remarks> The prototype chain is not searched. </remarks>
        public virtual PropertyDescriptor GetOwnPropertyDescriptor(uint index)
        {
            var property = this.schema.GetPropertyIndexAndAttributes(index.ToString());
            if (property.Exists == true)
                return new PropertyDescriptor(this.propertyValues[property.Index], property.Attributes);
            return PropertyDescriptor.Undefined;
        }

        /// <summary>
        /// Gets a descriptor for the property with the given name.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <returns> A property descriptor containing the property value and attributes. </returns>
        /// <remarks> The prototype chain is not searched. </remarks>
        public PropertyDescriptor GetOwnPropertyDescriptor(string propertyName)
        {
            // Check if the property is an indexed property.
            uint arrayIndex = ArrayInstance.ParseArrayIndex(propertyName);
            if (arrayIndex != uint.MaxValue)
                return GetOwnPropertyDescriptor(arrayIndex);

            // Retrieve information about the property.
            var property = this.schema.GetPropertyIndexAndAttributes(propertyName);
            if (property.Exists == true)
            {
                if (property.IsLength == false)
                    return new PropertyDescriptor(this.propertyValues[property.Index], property.Attributes);

                // The property is the "magic" length property.
                return new PropertyDescriptor((int)((ArrayInstance)this).Length, property.Attributes);
            }

            // The property doesn't exist.
            return PropertyDescriptor.Undefined;
        }

        ///// <summary>
        ///// Determines if the property with the given name is writable.
        ///// </summary>
        ///// <param name="propertyName"> The name of the property. </param>
        ///// <returns> <c>true</c> if a property with the given name is writable; <c>false</c>
        ///// otherwise. </returns>
        //internal bool CanPut(string propertyName)
        //{
        //    // Retrieve the property on this object.
        //    PropertyDescriptor descriptor = GetOwnProperty(propertyName);
        //    if (descriptor.Exists == true)
        //        return descriptor.IsWritable;

        //    // The property doesn't exist on this object.  A new property would normally be
        //    // created, unless an accessor function exists in the prototype chain.
        //    if (this.prototype != null)
        //    {
        //        descriptor = this.prototype.GetProperty(propertyName);
        //        if (descriptor.IsAccessor == true)
        //            return descriptor.SetAccessor != null;
        //    }

        //    // A new property can be created if the object is extensible.
        //    return this.IsExtensible;
        //}

        /// <summary>
        /// Sets the value of the property with the given array index.  If a property with the
        /// given index does not exist, or exists in the prototype chain (and is not a setter) then
        /// a new property is created.
        /// </summary>
        /// <param name="index"> The array index of the property to set. </param>
        /// <param name="value"> The value to set the property to.  This must be a javascript
        /// primitive (double, string, etc) or a class derived from <see cref="ObjectInstance"/>. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is read-only or if the object is sealed. </param>
        public virtual void SetPropertyValue(uint index, object value, bool throwOnError)
        {
            string indexStr = index.ToString();
            bool exists = SetPropertyValueIfExists(indexStr, value, throwOnError);
            if (exists == false)
            {
                // The property doesn't exist - add it.
                AddProperty(indexStr, value, PropertyAttributes.FullAccess, throwOnError);
            }
        }

        /// <summary>
        /// Sets the value of the property with the given name.  If a property with the given name
        /// does not exist, or exists in the prototype chain (and is not a setter) then a new
        /// property is created.
        /// </summary>
        /// <param name="propertyName"> The name of the property to set. </param>
        /// <param name="value"> The value to set the property to.  This must be a javascript
        /// primitive (double, string, etc) or a class derived from <see cref="ObjectInstance"/>. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set (i.e. if the property is read-only or if the object is not extensible and a new
        /// property needs to be created). </param>
        public void SetPropertyValue(string propertyName, object value, bool throwOnError)
        {
            // Check if the property is an indexed property.
            uint arrayIndex = ArrayInstance.ParseArrayIndex(propertyName);
            if (arrayIndex != uint.MaxValue)
            {
                SetPropertyValue(arrayIndex, value, throwOnError);
                return;
            }

            bool exists = SetPropertyValueIfExists(propertyName, value, throwOnError);
            if (exists == false)
            {
                // The property doesn't exist - add it.
                AddProperty(propertyName, value, PropertyAttributes.FullAccess, throwOnError);
            }
        }

        /// <summary>
        /// Sets the value of the given property.  If a property with the given name exists, but
        /// only in the prototype chain, then a new property is created (unless the property is a
        /// setter, in which case the setter is called and no property is created).  If the
        /// property does not exist at all, then no property is created and the method returns
        /// <c>false</c>.  This method is used to set the value of a variable reference within a
        /// with statement.
        /// </summary>
        /// <param name="name"> The name of the property to set.  Cannot be an array index. </param>
        /// <param name="value"> The desired value of the property.  This must be a javascript
        /// primitive (double, string, etc) or a class derived from <see cref="ObjectInstance"/>. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set (i.e. if the property is read-only or if the object is not extensible and a new
        /// property needs to be created). </param>
        /// <returns> <c>true</c> if the property value exists; <c>false</c> otherwise. </returns>
        public bool SetPropertyValueIfExists(string propertyName, object value, bool throwOnError)
        {
            // Do not store nulls - null represents a non-existant value.
            value = value ?? Undefined.Value;

            // Retrieve information about the property.
            var property = this.schema.GetPropertyIndexAndAttributes(propertyName);
            if (property.Exists == true)
            {
                // Check if the property is read-only.
                if (property.IsWritable == false)
                {
                    // The property is read-only.
                    if (throwOnError == true)
                        throw new JavaScriptException("TypeError", string.Format("The property '{0}' is read-only.", propertyName));
                    return true;
                }

                if ((property.Attributes & (PropertyAttributes.IsAccessorProperty | PropertyAttributes.IsLengthProperty)) == 0)
                {
                    // The property contains a simple value.  Set the property value.
                    this.propertyValues[property.Index] = value;
                }
                else if (property.IsAccessor == true)
                {
                    // The property contains an accessor function.  Set the property value by calling the accessor.
                    ((PropertyAccessorValue)this.propertyValues[property.Index]).SetValue(this, value);
                }
                else
                {
                    // Otherwise, the property is the "magic" length property.
                    double length = TypeConverter.ToNumber(value);
                    if (length < 0 || length > uint.MaxValue)
                        throw new JavaScriptException("RangeError", "Invalid array length");
                    ((ArrayInstance)this).Length = TypeConverter.ToUint32(length);
                }
                return true;
            }

            // Search the prototype chain for a accessor function.  If one is found, it will
            // prevent the creation of a new property.
            bool propertyExistsInPrototype = false;
            ObjectInstance prototypeObject = this.prototype;
            while (prototypeObject != null)
            {
                property = prototypeObject.schema.GetPropertyIndexAndAttributes(propertyName);
                if (property.Exists == true)
                {
                    if (property.IsAccessor == true)
                    {
                        // The property contains an accessor function.  Set the property value by calling the accessor.
                        ((PropertyAccessorValue)prototypeObject.propertyValues[property.Index]).SetValue(this, value);
                        return true;
                    }
                    propertyExistsInPrototype = true;
                    break;
                }
                prototypeObject = prototypeObject.prototype;
            }

            // If the property exists in the prototype, create a new property.
            if (propertyExistsInPrototype == true)
            {
                AddProperty(propertyName, value, PropertyAttributes.FullAccess, throwOnError);
                return true;
            }

            // The property does not exist.
            return false;
        }
        
        /// <summary>
        /// Deletes the property with the given array index.
        /// </summary>
        /// <param name="index"> The array index of the property to delete. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set because the property was marked as non-configurable.  </param>
        /// <returns> <c>true</c> if the property was successfully deleted, or if the property did
        /// not exist; <c>false</c> if the property was marked as non-configurable and
        /// <paramref name="throwOnError"/> was <c>false</c>. </returns>
        public virtual bool Delete(uint index, bool throwOnError)
        {
            string indexStr = index.ToString();

            // Retrieve the attributes for the property.
            var propertyInfo = this.schema.GetPropertyIndexAndAttributes(indexStr);
            if (propertyInfo.Exists == false)
                return true;    // Property doesn't exist - delete succeeded!

            // Delete the property.
            this.schema = this.schema.DeleteProperty(indexStr);
            return true;
        }

        /// <summary>
        /// Deletes the property with the given name.
        /// </summary>
        /// <param name="propertyName"> The name of the property to delete. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set because the property was marked as non-configurable.  </param>
        /// <returns> <c>true</c> if the property was successfully deleted, or if the property did
        /// not exist; <c>false</c> if the property was marked as non-configurable and
        /// <paramref name="throwOnError"/> was <c>false</c>. </returns>
        public bool Delete(string propertyName, bool throwOnError)
        {
            // Check if the property is an indexed property.
            uint arrayIndex = ArrayInstance.ParseArrayIndex(propertyName);
            if (arrayIndex != uint.MaxValue)
                return Delete(arrayIndex, throwOnError);

            // Retrieve the attributes for the property.
            var propertyInfo = this.schema.GetPropertyIndexAndAttributes(propertyName);
            if (propertyInfo.Exists == false)
                return true;    // Property doesn't exist - delete succeeded!

            // Check if the property can be deleted.
            if (propertyInfo.IsConfigurable == false)
            {
                if (throwOnError == true)
                    throw new JavaScriptException("TypeError", string.Format("The property '{0}' cannot be deleted.", propertyName));
                return false;
            }

            // Delete the property.
            this.schema = this.schema.DeleteProperty(propertyName);
            return true;
        }

        /// <summary>
        /// Defines or redefines the value and attributes of a property.  The prototype chain is
        /// not searched so if the property exists but only in the prototype chain a new property
        /// will be created.
        /// </summary>
        /// <param name="propertyName"> The name of the property to modify. </param>
        /// <param name="descriptor"> The property value and attributes. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be set.  This can happen if the property is not configurable or the object is sealed. </param>
        /// <returns> <c>true</c> if the property was successfully modified; <c>false</c> otherwise. </returns>
        public virtual bool DefineProperty(string propertyName, PropertyDescriptor descriptor, bool throwOnError)
        {
            // Retrieve info on the property.
            var current = this.schema.GetPropertyIndexAndAttributes(propertyName);

            if (current.Exists == false)
            {
                // Create a new property.
                return AddProperty(propertyName, descriptor.Value, descriptor.Attributes, throwOnError);
            }

            // If the current property is not configurable, then the only change that is allowed is
            // a change from one simple value to another (i.e. accessors are not allowed) and only
            // if the writable attribute is set.
            if (current.IsConfigurable == false)
            {
                // Get the current value of the property.
                object currentValue = this.propertyValues[current.Index];
                object getter = null, setter = null;
                if (currentValue is PropertyAccessorValue)
                {
                    getter = ((PropertyAccessorValue)currentValue).Getter;
                    setter = ((PropertyAccessorValue)currentValue).Setter;
                }

                // Check if the modification is allowed.
                if (descriptor.Attributes != current.Attributes ||
                    (descriptor.IsAccessor == true && (getter != descriptor.Getter || setter != descriptor.Setter)) ||
                    (descriptor.IsAccessor == false && current.IsWritable == false && TypeComparer.SameValue(currentValue, descriptor.Value) == false))
                {
                    if (throwOnError == true)
                        throw new JavaScriptException("TypeError", string.Format("The property '{0}' is non-configurable.", propertyName));
                    return false;
                }
            }

            // Set the property value and attributes.
            this.schema = this.schema.SetPropertyAttributes(propertyName, descriptor.Attributes);
            this.propertyValues[current.Index] = descriptor.Value;

            return true;
        }

        /// <summary>
        /// Adds a property to this object.  The property must not already exist.
        /// </summary>
        /// <param name="propertyName"> The name of the property to add. </param>
        /// <param name="value"> The desired value of the property.  This can be a
        /// <see cref="PropertyAccessorValue"/>. </param>
        /// <param name="attributes"> Attributes describing how the property may be modified. </param>
        /// <param name="throwOnError"> <c>true</c> to throw an exception if the property could not
        /// be added (i.e. if the object is not extensible). </param>
        /// <returns> <c>true</c> if the property was successfully added; <c>false</c> otherwise. </returns>
        private bool AddProperty(string propertyName, object value, PropertyAttributes attributes, bool throwOnError)
        {
            // Make sure adding a property is allowed.
            if (this.IsExtensible == false)
            {
                if (throwOnError == true)
                    throw new JavaScriptException("TypeError", string.Format("The property '{0}' cannot be created as the object is not extensible.", propertyName));
                return false;
            }

            // Do not store nulls - null represents a non-existant value.
            value = value ?? Undefined.Value;

            // Add a new property to the schema.
            this.schema = this.schema.AddProperty(propertyName, attributes);

            // Check if the value array needs to be resized.
            int propertyIndex = this.schema.PropertyCount - 1;
            if (propertyIndex >= this.InlinePropertyValues.Length)
                Array.Resize(ref this.propertyValues, this.InlinePropertyValues.Length * 2);

            // Set the value of the property.
            this.propertyValues[propertyIndex] = value;

            // Success.
            return true;
        }

        /// <summary>
        /// Sets a property value and attributes, or adds a new property if it doesn't already
        /// exist.  Any existing attributes are ignored (and not modified).
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <param name="value"> The intended value of the property. </param>
        /// <param name="attributes"> Attributes that indicate whether the property is writable,
        /// configurable and enumerable. </param>
        /// <param name="overwriteAttributes"> Indicates whether to overwrite any existing attributes. </param>
        internal void FastSetProperty(string propertyName, object value, PropertyAttributes attributes = PropertyAttributes.Sealed, bool overwriteAttributes = false)
        {
            int index = this.schema.GetPropertyIndex(propertyName);
            if (index < 0)
            {
                // The property is doesn't exist - add a new property.
                AddProperty(propertyName, value, attributes, false);
                return;
            }
            if (overwriteAttributes == true)
                this.schema = this.schema.SetPropertyAttributes(propertyName, attributes);
            this.propertyValues[index] = value;
        }



        //     OTHERS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a primitive value that represents the current object.  Used by the addition and
        /// equality operators.
        /// </summary>
        /// <param name="hint"> Indicates the preferred type of the result. </param>
        /// <returns> A primitive value that represents the current object. </returns>
        protected internal virtual object GetPrimitiveValue(PrimitiveTypeHint typeHint)
        {
            if (typeHint == PrimitiveTypeHint.None || typeHint == PrimitiveTypeHint.Number)
            {

                // Try calling valueOf().
                object valueOfResult;
                if (TryCallMemberFunction(out valueOfResult, "valueOf") == true)
                {
                    // Return value must be primitive.
                    if (valueOfResult is double || TypeUtilities.IsPrimitiveType(valueOfResult.GetType()) == true)
                        return valueOfResult;
                }

                // Try calling toString().
                object toStringResult;
                if (TryCallMemberFunction(out toStringResult, "toString") == true)
                {
                    // Return value must be primitive.
                    if (toStringResult is string || TypeUtilities.IsPrimitiveType(toStringResult.GetType()) == true)
                        return toStringResult;
                }

            }
            else
            {

                // Try calling toString().
                object toStringResult;
                if (TryCallMemberFunction(out toStringResult, "toString") == true)
                {
                    // Return value must be primitive.
                    if (toStringResult is string || TypeUtilities.IsPrimitiveType(toStringResult.GetType()) == true)
                        return toStringResult;
                }

                // Try calling valueOf().
                object valueOfResult;
                if (TryCallMemberFunction(out valueOfResult, "valueOf") == true)
                {
                    // Return value must be primitive.
                    if (valueOfResult is double || TypeUtilities.IsPrimitiveType(valueOfResult.GetType()) == true)
                        return valueOfResult;
                }

            }

            throw new JavaScriptException("TypeError", "Attempted conversion of the object to a primitive value failed.  Check the toString() and valueOf() functions.");
        }

        /// <summary>
        /// Calls the function with the given name.  The function must exist on this object or an
        /// exception will be thrown.
        /// </summary>
        /// <param name="functionName"> The name of the function to call. </param>
        /// <param name="parameters"> The parameters to pass to the function. </param>
        /// <returns> The result of calling the function. </returns>
        public object CallMemberFunction(string functionName, params object[] parameters)
        {
            var function = GetPropertyValue(functionName);
            if (function == null)
                throw new JavaScriptException("TypeError", string.Format("Object {0} has no method '{1}'", this.ToString(), functionName));
            if ((function is FunctionInstance) == false)
                throw new JavaScriptException("TypeError", string.Format("Property '{1}' of object {0} is not a function", this.ToString(), functionName));
            return ((FunctionInstance)function).CallLateBound(this, parameters);
        }

        /// <summary>
        /// Calls the function with the given name.
        /// </summary>
        /// <param name="result"> The result of calling the function. </param>
        /// <param name="functionName"> The name of the function to call. </param>
        /// <param name="parameters"> The parameters to pass to the function. </param>
        /// <returns> <c>true</c> if the function was called successfully; <c>false</c> otherwise. </returns>
        public bool TryCallMemberFunction(out object result, string functionName, params object[] parameters)
        {
            var function = GetPropertyValue(functionName);
            if ((function is FunctionInstance) == false)
            {
                result = null;
                return false;
            }
            result = ((FunctionInstance)function).CallLateBound(this, parameters);
            return true;
        }

        /// <summary>
        /// Returns a string representing this object.
        /// </summary>
        /// <returns> A string representing this object. </returns>
        public override string ToString()
        {
            try
            {
                // Does a toString method/property exist?
                if (HasProperty("toString") == true)
                {
                    // Call the toString() method.
                    var result = CallMemberFunction("toString");

                    // Make sure we don't recursively loop forever.
                    if (result == this)
                        return "<error>";

                    // Convert to a string.
                    return TypeConverter.ToString(result);
                }
                else
                {
                    // Otherwise, return the type name.
                    var constructor = this["constructor"];
                    if (constructor is FunctionInstance)
                        return string.Format("[{0}]", ((FunctionInstance) constructor).Name);
                    return "<unknown>";
                }
            }
            catch (JavaScriptException)
            {
                return "<error>";
            }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Determines if a property with the given name exists on this object.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <returns> <c>true</c> if a property with the given name exists on this object,
        /// <c>false</c> otherwise. </returns>
        /// <remarks> Objects in the prototype chain are not considered. </remarks>
        [JSFunction(Name = "hasOwnProperty", Flags = FunctionBinderFlags.HasThisObject)]
        public static bool HasOwnProperty(object thisObject, string propertyName)
        {
            TypeUtilities.VerifyThisObject(thisObject, "hasOwnProperty");
            return TypeConverter.ToObject(thisObject).GetOwnPropertyDescriptor(propertyName).Exists;
        }

        /// <summary>
        /// Determines if this object is in the prototype chain of the given object.
        /// </summary>
        /// <param name="obj"> The object to check. </param>
        /// <returns> <c>true</c> if this object is in the prototype chain of the given object;
        /// <c>false</c> otherwise. </returns>
        [JSFunction(Name = "isPrototypeOf", Flags = FunctionBinderFlags.HasThisObject)]
        public static bool IsPrototypeOf(object thisObject, object obj)
        {
            if ((obj is ObjectInstance) == false)
                return false;
            TypeUtilities.VerifyThisObject(thisObject, "isPrototypeOf");
            var obj2 = obj as ObjectInstance;
            while (true)
            {
                obj2 = obj2.Prototype;
                if (obj2 == null)
                    return false;
                if (obj2 == thisObject)
                    return true;
            }
        }

        /// <summary>
        /// Determines if a property with the given name exists on this object and is enumerable.
        /// </summary>
        /// <param name="propertyName"> The name of the property. </param>
        /// <returns> <c>true</c> if a property with the given name exists on this object and is
        /// enumerable, <c>false</c> otherwise. </returns>
        /// <remarks> Objects in the prototype chain are not considered. </remarks>
        [JSFunction(Name = "propertyIsEnumerable", Flags = FunctionBinderFlags.HasThisObject)]
        public static bool PropertyIsEnumerable(object thisObject, string propertyName)
        {
            TypeUtilities.VerifyThisObject(thisObject, "propertyIsEnumerable");
            var property = TypeConverter.ToObject(thisObject).GetOwnPropertyDescriptor(propertyName);
            return property.Exists && property.IsEnumerable;
        }

        /// <summary>
        /// Returns a locale-dependant string representing the current object.
        /// </summary>
        /// <returns> Returns a locale-dependant string representing the current object. </returns>
        [JSFunction(Name = "toLocaleString")]
        public string ToLocaleString()
        {
            return TypeConverter.ToString(CallMemberFunction("toString"));
        }

        /// <summary>
        /// Returns a primitive value associated with the object.
        /// </summary>
        /// <returns> A primitive value associated with the object. </returns>
        [JSFunction(Name = "valueOf")]
        public ObjectInstance ValueOf()
        {
            return this;
        }

        /// <summary>
        /// Returns a string representing the current object.
        /// </summary>
        /// <param name="thisObject"> The value of the "this" keyword. </param>
        /// <returns> A string representing the current object. </returns>
        [JSFunction(Name = "toString", Flags = FunctionBinderFlags.HasThisObject)]
        public static string ToStringJS(object thisObject)
        {
            if (thisObject == null || thisObject == Undefined.Value)
                return "[object undefined]";
            if (thisObject == Null.Value)
                return "[object null]";
            return string.Format("[object {0}]", TypeConverter.ToObject(thisObject).InternalClassName);
        }



        //     ATTRIBUTE-BASED PROTOTYPE POPULATION
        //_________________________________________________________________________________________

        private class MethodGroup
        {
            public List<FunctionBinderMethod> Methods;
            public int Length;
        }

        /// <summary>
        /// Populates the object with functions and properties.  Should be called only once at
        /// startup.
        /// </summary>
        internal protected void PopulateFunctions(Type type = null)
        {
            if (type == null)
                type = this.GetType();
            
            // Group the methods on the given type by name.
            var functions = new Dictionary<string, MethodGroup>(20);
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                // Make sure the method has the [JSFunction] attribute.
                var attribute = (JSFunctionAttribute)Attribute.GetCustomAttribute(method, typeof(JSFunctionAttribute));
                if (attribute == null)
                    continue;

                // Determine the name of the method.
                string name;
                if (attribute.Name != null)
                {
                    name = Compiler.Lexer.ResolveIdentifier(attribute.Name);
                    if (name == null)
                        throw new InvalidOperationException(string.Format("The name provided to [JSFunction] on {0} is not a valid identifier.", method));
                }
                else
                    name = method.Name;

                // Get a reference to the method group.
                MethodGroup methodGroup;
                if (functions.ContainsKey(name) == false)
                {
                    methodGroup = new MethodGroup { Methods = new List<FunctionBinderMethod>(1), Length = -1 };
                    functions.Add(name, methodGroup);
                }
                else
                    methodGroup = functions[name];

                // Add the method to the list.
                methodGroup.Methods.Add(new FunctionBinderMethod(method, attribute.Flags));

                // If the length doesn't equal -1, that indicates an explicit length has been set.
                // Make sure it is consistant with the other methods.
                if (attribute.Length >= 0)
                {
                    if (methodGroup.Length != -1 && methodGroup.Length != attribute.Length)
                        throw new InvalidOperationException(string.Format("Inconsistant Length property detected on {0}.", method));
                    methodGroup.Length = attribute.Length;
                }
            }

            // Now set the relevant properties on the object.
            foreach (KeyValuePair<string, MethodGroup> pair in functions)
            {
                string name = pair.Key;
                MethodGroup methodGroup = pair.Value;

                // Add the function as a property of the object.
                this.FastSetProperty(name, new ClrFunction(GlobalObject.Function.InstancePrototype, methodGroup.Methods, name, methodGroup.Length));
            }
        }

        /// <summary>
        /// Populates the object with functions and properties.  Should be called only once at
        /// startup.
        /// </summary>
        internal protected void PopulateFields(Type type = null)
        {
            if (type == null)
                type = this.GetType();

            // Find all fields with [JsField]
            foreach (var field in type.GetFields())
            {
                var attribute = (JSFieldAttribute)Attribute.GetCustomAttribute(field, typeof(JSFieldAttribute));
                if (attribute == null)
                    continue;
                this.FastSetProperty(field.Name, field.GetValue(this));
            }
        }
    }
}