# Blazored Gitter

A Gitter client written in Blazor

[![Build Status](https://dev.azure.com/blazored/Gitter/_apis/build/status/Blazored.Gitter?branchName=master)](https://dev.azure.com/blazored/Gitter/_build/latest?definitionId=8&branchName=master)

This is a work in progress, not production ready.

## Get Involved

You will need Visual Studio 2019 16.4.0 or newer.

(VS Code and VS Mac are gaining support — you may find those work as well.)

You will need to download and install the [.Net Core SDK 3.1.102 / runtime 3.1.2](https://dotnet.microsoft.com/download/dotnet-core/3.1).

Fork the repo, open and run the `Blazor.Gitter.Client` project.

To log in, you will need a Gitter API Token, which you can get by visiting https://developer.gitter.im/docs/welcome
and signing in using your normal Gitter credentials.

Once signed in, your token is presented to you - you don't need to request one.

Of course, this is your own personal token - don't share it.

Once you are up and running, take a look at the issues list and see if you can help.

**Note** There is currently an issue with a couple of Gitter APIs that don't work in a purely client-based application like this. The streaming API is one, so we have to poll for new messages. The other is Authentication, which is why, for now at least, we are using personal tokens. 
