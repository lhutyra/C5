/*
 Copyright (c) 2003-2017 Niels Kokholm, Peter Sestoft, and Rasmus Lystr�m
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
*/

using System;
using System.Reflection;
using C5;
using NUnit.Framework;
using SCG = System.Collections.Generic;

namespace C5UnitTests.Templates
{
    public abstract class GenericCollectionTester<U, W>
    {
        protected CircularQueue<MethodInfo> testMethods;
        public GenericCollectionTester()
        {
            testMethods = new CircularQueue<MethodInfo>();
            foreach (MethodInfo minfo in this.GetType().GetMethods())
            {
                if (minfo.GetParameters().Length == 0 &&
                     minfo.GetCustomAttributes(typeof(TestAttribute), false).Length > 0)
                    testMethods.Enqueue(minfo);
            }
        }

        public virtual void Test(Func<U> factory, MemoryType memoryType = MemoryType.Normal)
        {
            foreach (MethodInfo minfo in testMethods)
            {
                foreach (W testSpec in GetSpecs())
                {
                    SetUp(factory(), testSpec, memoryType);
                    //Console.WriteLine("Testing {0}, with method {1} and testSpec {{{2}}}", typeof(U), minfo.Name, testSpec);
                    try
                    {
                        minfo.Invoke(this, null);
                    }
                    catch (TargetInvocationException)
                    {
                        //if (e.InnerException is ExpectedExceptionAttribute)
                        //{
                        //}
                        //else
                        throw;
                    }
                    //tearDown
                }
            }
        }

        public abstract void SetUp(U collection, W testSpec, MemoryType memoryType);
        public abstract SCG.IEnumerable<W> GetSpecs();
    }

    public abstract class GenericCollectionTester<U> : GenericCollectionTester<U, int>
    {
        public override System.Collections.Generic.IEnumerable<int> GetSpecs()
        {
            return new int[] { 0 };
        }

        public override void SetUp(U collection, int testSpec, MemoryType memoryType)
        {
            SetUp(collection, memoryType);
        }

        public abstract void SetUp(object collection, MemoryType memoryType);
    }
}