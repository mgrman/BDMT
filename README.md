# Blazor Dual Mode Template

* Dual mode
  * Server-side
  * Client-side
  * Client-side with prerendering
*  Authentication 
  * Common for both modes using `Microsoft.AspNetCore.Identity`
  * User login persists changing mode
  * Cookie based
* Common Services
  * Same interface used in both modes
  * Server-side the interface can be used directly
    * Guard class is generated to still validate Authentication attribute (using custom Code Generator to create the classes)
  * Client-side the interface is implemented by Code-first GRPC services 
    * GRPC service are automatically generated (using protobuf-net.Grpc) from the server side implementation of the interface
  * Needs to be marked with `[ServiceContract]` and data types need to be DataContract serializable.

## Demo

[Demo available here.](https://bdmt.herokuapp.com/)