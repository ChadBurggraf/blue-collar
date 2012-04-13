//-----------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Blue Collar SQLite Repository")]
[assembly: AssemblyDescription("Blue Collar SQLite repository plugin.")]
[assembly: Guid("220d4a3a-c8db-4a0b-9db1-7a7a2b20c39e")]

[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lite")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "BlueCollar")]

#if DEBUG
[assembly: InternalsVisibleTo("BlueCollar.Test")]
#endif