//-----------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Blue Collar Service")]
[assembly: AssemblyDescription("Blue Collar execution service.")]
[assembly: Guid("af1468d7-3cd3-42c4-8dce-035589a22cfe")]

#if DEBUG
[assembly: InternalsVisibleTo("BlueCollar.Test")]
#endif