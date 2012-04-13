//-----------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Tasty Codes">
//     Copyright (c) 2011 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Blue Collar")]
[assembly: AssemblyDescription("Blue Collar core library.")]
[assembly: Guid("3376b73e-a3fa-4351-859c-f5909aea833a")]

#if DEBUG
[assembly: InternalsVisibleTo("BlueCollar.Dashboard")]
[assembly: InternalsVisibleTo("BlueCollar.Test")]
#endif