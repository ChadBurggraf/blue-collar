# Blue Collar
#### Background jobs and scheduled work for .NET.

Blue Collar is a system for creating, managing, and executing background work in .NET applications. It is ideal for use in web applications, but can be set up for any environment where background work needs to be done.

Some cool features include:

  * Simple, single-assembly deployment, with no external dependencies.
  * Embedded web-based management interface.
  * Easily perform work for multiple applications on one machine, or perform work for one application on multiple machines, or any combination in between.
  * Jobs are simple .NET objects, implementing `string Name { get; }` and `void Execute()`.
  * Sensible defaults make getting up and running quickly a breeze.

## The "Just Let Me Play With It" Setup

For the quick setup, we'll use [SQLite](http://www.sqlite.org/) as our data store. Before starting, install the appropriate [System.Data.SQLite.dll](http://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki). If you choose **not** to install `System.Data.SQLite` in the GAC, add a reference to the DLL to your project.

Next, add references to `BlueCollar.dll` and `BlueCollar.SQLiteRepository.dll` to your project. Builds are provided for both .NET 3.5 and .NET 4.0 applications. Note that data store implementations are distributed as separate assemblies, except for Microsoft SQL Server, which is included in `BlueCollar.dll`.

#### Configuration

Once your references are added, you need to do a little (promise!) configuration. At a minimum, you need to tell Blue Collar the name of your application. We're also going to configure the web management dashboard by registering its handler.

First, register the `blueCollar` configuration section under `<configSections/>`:

    <configSections>
      <section name="blueCollar" type="BlueCollar.BlueCollarSection, BlueCollar" allowDefinition="Everywhere" allowLocation="true" requirePermission="false"/>
    </configSections>

Now add the `blueCollar` section as a child of the root `<configuration/>` element:

    <blueCollar applicationName="Your Application Name"/>

Replace **Your Application Name** with a friendly application name. The system can accomodate multiple applications in the same database schema, so the application name you choose will also be used to partition this application's data.

Finally, register the web management dashboard handler. By default, the handler is configured for registration at `~/collar`, so that's where we'll register it under `<system.webServer/>`:

    <system.webServer>
      <handlers>
        <add name="collar" path="collar" type="BlueCollar.Dashboard.DashboardHandlerFactory, BlueCollar" verb="*" preCondition="integratedMode" />
      </handlers>
    </system.webServer>

#### Creating a Machine In-Process

Blue Collar can create a `Machine` (our name for the service used to coordinate workers on a computer for an application) inside of the ASP.NET process. This ability is useful for low-volume scenarios, development environments, and getting up and running quickly.

To mange this in a flexible way, we'll boot up our `Machine` on application start, but only if we've configured execution by the service to be **disabled** (the default). That way, when we want to switch to service-based execution, we just have to make a quick configuration update.

If your application doesn't have one already, create a `Global.asax` file and add `using BlueCollar;` to your list of namespace registrations.

Next, create a class variable called `machine`:

    private Machine machine;

And add or update `void Application_Start()`:

    void Application_Start()
    {
        if (!BlueCollarSection.Section.Machine.ServiceExecutionEnabled)
        {
            machine = new Machine();
        }
    }

Finally, to stop work when the application is shut down, add or update `void Application_End()`:

    void Application_End()
    {
    	if (machine != null) 
    	{
            machine.Dispose();
            machine = null;
    	}
    }

#### That's It!

Build your application and launch it in the browser. You should be able to visit `~/collar` and see that the default worker has been created and is running.

## Creating Jobs

To do actual work, you need to create some jobs. Usually, this is a matter of converting an existing method invocation into a `Job` class. 

For example, let's say we wanted to create a job for sending email. Suppose we currently do it by calling a static method on an `EmailService` class with the following signature:

    void Send(string address, string subject, string body);

In order to do this work from a job, we might create a class as follows:

    using System;
    using BlueCollar;

    public sealed class SendEmailJob : Job
    {
    	public string Address { get; set; }

    	public string Body { get; set; }

    	public override string Name
    	{
    		get { return "Send Email"; }
    	}

    	public string Subject { get; set; }

    	public override void Execute()
    	{
    		EmailService.Send(this.Address, this.Subject, this.Body);
    	}
    }

All we did was create properties for the state we needed in order to perform the work. When we enqueue the job, our object will be serialized onto the job queue with all of our custom state included. When the job is later materialized for execution, the state will be de-serialized and available for use.

Now, in order to send an actual email, we'd do something similar to the following:

    new SendEmailJob()
    {
        Address = "example@example.com",
        Subject = "Hello, World!",
        Body = "This email was sent in the background, freeing up my application to be more responsive!"
    }.Enqueue();

## License

Licensed under the [MIT](http://www.opensource.org/licenses/mit-license.html) license.

Copyright (c) 2012 Chad Burggraf.