# AzureFuncPOC - URL Shortening service

The service uses Azure Functions. I never used them, so I decided to try them.
The service project is written in .NET Core 2.2 and uses Cosmos Table API for storing the links and index.
The simple API consists of 3 functions:

  FunctionShorten   - Returns a short version of an input url    
  FunctionRedirect  - Redirects browser to a full URL when a short version is provided    
  FunctionInflate   - Returns a full URL when a short version is provided   

Azure Examples:

  https://aros057.azurewebsites.net/api/FunctionShorten?UrlFull=https://www.google.com
  
  https://aros057.azurewebsites.net/api/FunctionInflate?UrlShort=1q
  
  https://aros057.azurewebsites.net/api/FunctionRedirect?UrlShort=1q

Examples(running on a local machine):

    http://localhost:7071/api/FunctionShorten?UrlFull=https://www.google.com
    response(plain text):
    localhost:7071/api/FunctionRedirect?UrlShort=c
    Where 'c' is the token that is calculated by the FunctionShorten by converting incrementing index into base 62 (digits + high/low case  alpha)
    Naturally, the lenght of the token will grow over time.
    
    
    http://localhost:7071/api/FunctionInflate?UrlShort=localhost:7071/api/FunctionRedirect?UrlShort=c
    response(plain text):
    https://www.google.com
    
  Publishing to Azure. Continuos delivery enabled for these projects to publish to Azure automatically.
  Alternatively, publish from Visual Studio.
 

  
  # Web Application
  Front end is a standard ASP .NET Core Web Application. The API is consumed directly by the JavaScript in the main page:
  Pages\Index.cshtml
  Web Application URL:  http://aros057tiny.azurewebsites.net/
  
  # Unit test project
  XUnit framework used for unit tests. In fact, these are integration tests.
    
  # Limitations & Improvements
  FunctionShorten does not checks the database whether input URL already exists in short form.
  
  Full URLs must include http or https prefix. I am planning to append http if it is not present (like tinyUrl does)
  
  Service names are way too long. So our service is not much of a shortener. Names and URLs for the service can be mapped by proxy to form a small URL, however this requires paid Azure account
  
  
  
  
