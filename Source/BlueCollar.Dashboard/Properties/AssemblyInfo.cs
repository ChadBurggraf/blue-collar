//-----------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Blue Collar Dashboard")]
[assembly: AssemblyDescription("Blue Collar web dashboard.")]
[assembly: Guid("f6b60f5f-9f15-41e1-a370-08b36b98ab65")]

#if DEBUG
[assembly: InternalsVisibleTo("BlueCollar.Test")]
#endif

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "BlueCollar.Dashboard", Justification = "MVC conventions.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "BlueCollar.Dashboard.Controllers", Justification = "MVC conventions.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "BlueCollar.Dashboard.Models", Justification = "MVC conventions.")]