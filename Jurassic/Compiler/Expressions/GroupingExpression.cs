﻿using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents a grouping expression.
    /// </summary>
    internal sealed class GroupingExpression : OperatorExpression
    {
        /// <summary>
        /// Creates a new instance of GroupingJSExpression.
        /// </summary>
        /// <param name="operator"> The operator to base this expression on. </param>
        public GroupingExpression(Operator @operator)
            : base(@operator)
        {
        }

        /// <summary>
        /// Gets the expression inside the grouping operator.
        /// </summary>
        public Expression Operand
        {
            get { return this.GetOperand(0); }
        }

        /// <summary>
        /// Gets the type that results from evaluating this expression.
        /// </summary>
        public override PrimitiveType ResultType
        {
            get { return this.Operand.ResultType; }
        }

        /// <summary>
        /// Generates CIL for the expression.
        /// </summary>
        /// <param name="generator"> The generator to output the CIL to. </param>
        /// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
        protected override void GenerateCodeCore(ILGenerator generator, OptimizationInfo optimizationInfo)
        {
            this.Operand.GenerateCode(generator, optimizationInfo);
        }
    }

}