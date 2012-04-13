//-----------------------------------------------------------------------
// <copyright file="ExamplesTests.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using BlueCollar.Examples;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Examples tests.
    /// </summary>
    [TestClass]
    public sealed class ExamplesTests
    {
        /// <summary>
        /// Create file job tests.
        /// </summary>
        [TestMethod]
        public void ExamplesCreateFileJob()
        {
            CreateFileJob job = new CreateFileJob();
            Assert.IsFalse(File.Exists(job.Path));
            job.Execute();
            Assert.IsTrue(File.Exists(job.Path));
        }

        /// <summary>
        /// Tail ABC tests.
        /// </summary>
        [TestMethod]
        public void ExamplesTailAbc()
        {
            string path = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt";
            ApplicationUtils.CopyEmbeddedResourceToPath("TailTest-Abc.txt", path);

            IList<string> tail = Tail.Read(path, 0, Encoding.UTF8).ToList();
            Assert.AreEqual(2, tail.Count);
            Assert.AreEqual("Fghi", tail[0]);
            Assert.AreEqual("Abcde", tail[1]);
        }

        /// <summary>
        /// Tail long lines tests.
        /// </summary>
        [TestMethod]
        public void ExamplesTailLongLines()
        {
            string path = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt";
            ApplicationUtils.CopyEmbeddedResourceToPath("TailTest-LongLines.txt", path);

            IList<string> tail = Tail.Read(path, 1, Encoding.UTF8).ToList();
            Assert.AreEqual(1, tail.Count);
            Assert.AreEqual("Cras ultrices, risus ut dictum suscipit, felis diam porta risus, non commodo eros neque vitae leo. Suspendisse condimentum suscipit urna, eget iaculis risus tempus vitae. Praesent luctus, magna ac feugiat euismod, enim dolor mattis sapien, eu mattis libero arcu vitae sem. Fusce eu erat ac metus gravida sodales a id tellus. Donec lacinia facilisis commodo. Vestibulum rhoncus aliquam arcu a varius. Pellentesque elementum egestas dictum. Ut orci nibh, imperdiet eget accumsan quis, pellentesque vel orci. Aliquam aliquam odio eu nulla bibendum mollis. Praesent enim turpis, volutpat at molestie vel, tincidunt id neque. In hac habitasse platea dictumst. Sed sit amet sapien eros. Sed consequat sem eu nunc vulputate sollicitudin. Sed ut suscipit augue. Phasellus rhoncus mauris et odio vulputate a placerat velit vestibulum. Vivamus eleifend augue sit amet libero bibendum lacinia. In faucibus, ante vel commodo bibendum, erat mi venenatis diam, sit amet laoreet libero quam ac nibh. Quisque porta volutpat lectus a luctus. Curabitur euismod pellentesque diam a placerat. Fusce dui velit, mollis in porttitor et, ullamcorper eu lacus. Aliquam vel diam in lorem dictum pellentesque in imperdiet nisi. Proin pretium ipsum in est eleifend quis ultrices orci ultrices. Morbi facilisis, libero non vulputate adipiscing, purus purus fermentum orci, non porttitor neque arcu eu arcu. Quisque eget tortor quam. Aenean risus nunc, ornare quis luctus ut, condimentum nec leo. Suspendisse quis magna non dui semper facilisis a eu nisi.", tail[0]);
        }

        /// <summary>
        /// Tail split lines tests.
        /// </summary>
        [TestMethod]
        public void ExamplesTailSplitLines()
        {
            string path = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt";
            ApplicationUtils.CopyEmbeddedResourceToPath("TailTest-SplitLines.txt", path);

            IList<string> tail = Tail.Read(path, 3, Encoding.UTF8).ToList();
            Assert.AreEqual(3, tail.Count);
            Assert.AreEqual("Sed ut suscipit augue. Phasellus rhoncus mauris et odio vulputate a placerat velit vestibulum. Vivamus eleifend augue sit amet libero bibendum lacinia. In faucibus, ante vel commodo bibendum, erat mi venenatis diam, sit amet laoreet libero quam ac nibh. Quisque porta volutpat lectus a luctus. Curabitur euismod pellentesque diam a placerat. Fusce dui velit, mollis in porttitor et, ullamcorper eu lacus. Aliquam vel diam in lorem dictum pellentesque in imperdiet nisi. Proin pretium ipsum in est eleifend quis ultrices orci ultrices. Morbi facilisis, libero non vulputate adipiscing, purus purus fermentum orci, non porttitor neque arcu eu arcu. Quisque eget tortor quam. Aenean risus nunc, ornare quis luctus ut, condimentum nec leo. Suspendisse quis magna non dui semper facilisis a eu nisi.", tail[0]);
            Assert.AreEqual(string.Empty, tail[1]);
            Assert.AreEqual("Cras ultrices, risus ut dictum suscipit, felis diam porta risus, non commodo eros neque vitae leo. Suspendisse condimentum suscipit urna, eget iaculis risus tempus vitae. Praesent luctus, magna ac feugiat euismod, enim dolor mattis sapien, eu mattis libero arcu vitae sem. Fusce eu erat ac metus gravida sodales a id tellus. Donec lacinia facilisis commodo. Vestibulum rhoncus aliquam arcu a varius. Pellentesque elementum egestas dictum. Ut orci nibh, imperdiet eget accumsan quis, pellentesque vel orci. Aliquam aliquam odio eu nulla bibendum mollis. Praesent enim turpis, volutpat at molestie vel, tincidunt id neque. In hac habitasse platea dictumst. Sed sit amet sapien eros. Sed consequat sem eu nunc vulputate sollicitudin.", tail[2]);
        }

        /// <summary>
        /// Tail subset tests.
        /// </summary>
        [TestMethod]
        public void ExamplesTailSubset()
        {
            string path = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt";
            ApplicationUtils.CopyEmbeddedResourceToPath("TailTest-10.txt", path);

            IList<string> tail = Tail.Read(path, 3, Encoding.UTF8).ToList();
            Assert.AreEqual(3, tail.Count);
            Assert.AreEqual("Donec orci odio, vehicula ac convallis non, consectetur sed augue.", tail[0]);
            Assert.AreEqual("Mauris vitae nibh odio.", tail[1]);
            Assert.AreEqual("Mauris id diam ac nulla suscipit faucibus non non orci.", tail[2]);
        }

        /// <summary>
        /// Tail superset tests.
        /// </summary>
        [TestMethod]
        public void ExamplesTailSuperset()
        {
            string path = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt";
            ApplicationUtils.CopyEmbeddedResourceToPath("TailTest-10.txt", path);

            IList<string> tail = Tail.Read(path, 100, Encoding.UTF8).ToList();
            Assert.AreEqual(10, tail.Count);
            Assert.AreEqual("Donec orci odio, vehicula ac convallis non, consectetur sed augue.", tail[0]);
            Assert.AreEqual("Lorem ipsum dolor sit amet, consectetur adipiscing elit.", tail[9]);
        }
    }
}
