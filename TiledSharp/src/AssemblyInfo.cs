﻿#if !NETCOREAPP
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// Information about this assembly is defined by the following attributes.
// Change them to the values specific to your project.
[assembly: AssemblyTitle("TiledSharp")]
[assembly: AssemblyDescription(
@"C# library for parsing and importing TMX and TSX files generated by Tiled, a
tile map generation tool")]
[assembly: AssemblyCompany("Marshall Ward")]
[assembly: AssemblyProduct("TiledSharp")]
[assembly: AssemblyCopyright("Copyright 2012-2016 Marshall Ward")]
// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.
[assembly: AssemblyVersion("0.15.0.0")]
[assembly: AssemblyInformationalVersion("0.15.0.0")]
// The following attributes are used to specify the signing key for the assembly,
// if desired. See the Mono documentation for more information about signing.
//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]
// Assume these are true, until I hear otherwise
[assembly: CLSCompliant(false)]
[assembly: ComVisible(true)]
#endif