# Blue Collar
#### Background jobs and scheduled work for .NET.

Blue Collar is a system for creating, managing, and executing background work in .NET applications. It is ideal for use in web applications, but can be set up for any environment where robust, out-of-band work needs to be done.

Some cool features include:

  * Simple, single-assembly deployment, with no external dependencies.
  * Embedded web-based management interface.
  * Easily perform work for multiple applications on one machine, or perform work for one application on multiple machines, or any combination in between.
  * Jobs are simple .NET objects, implementing `string Name { get; }` and `void Execute()`.
  * Sensible defaults make getting up and running quickly a breeze.

## The "Just Let Me Play With It" Setup

For the quick setup, we'll use [SQLite](http://www.sqlite.org/) as our data store. Before starting, install the appropriate [System.Data.SQLite.dll](http://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki) and add a reference to the DLL to your project.