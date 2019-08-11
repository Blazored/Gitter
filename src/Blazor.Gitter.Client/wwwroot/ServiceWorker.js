const baseURL = '/';
const indexURL = '/index.html';
const networkFetchEvent = 'fetch';
const swInstallEvent = 'install';
const swInstalledEvent = 'installed';
const swActivateEvent = 'activate';
const staticCachePrefix = 'blazored-gitter-v';
const staticCacheName = 'blazored-gitter-v0.1.0-beta20190810120856';
const requiredFiles = [
"/_framework/blazor.boot.json",
"/_framework/blazor.server.js",
"/_framework/blazor.webassembly.js",
"/_framework/wasm/mono.js",
"/_framework/wasm/mono.wasm",
"/_framework/_bin/Blazor.Gitter.Client.dll",
"/_framework/_bin/Blazor.Gitter.Client.pdb",
"/_framework/_bin/Blazor.Gitter.Core.dll",
"/_framework/_bin/Blazor.Gitter.Core.pdb",
"/_framework/_bin/Blazor.Gitter.Library.dll",
"/_framework/_bin/Blazor.Gitter.Library.pdb",
"/_framework/_bin/BlazorComponentUtilities.dll",
"/_framework/_bin/Blazored.LocalStorage.dll",
"/_framework/_bin/BlazorEmbedLibrary.dll",
"/_framework/_bin/Microsoft.AspNetCore.Authorization.dll",
"/_framework/_bin/Microsoft.AspNetCore.Blazor.dll",
"/_framework/_bin/Microsoft.AspNetCore.Components.Browser.dll",
"/_framework/_bin/Microsoft.AspNetCore.Components.dll",
"/_framework/_bin/Microsoft.AspNetCore.Metadata.dll",
"/_framework/_bin/Microsoft.Bcl.AsyncInterfaces.dll",
"/_framework/_bin/Microsoft.Extensions.Configuration.Abstractions.dll",
"/_framework/_bin/Microsoft.Extensions.Configuration.Binder.dll",
"/_framework/_bin/Microsoft.Extensions.Configuration.dll",
"/_framework/_bin/Microsoft.Extensions.DependencyInjection.Abstractions.dll",
"/_framework/_bin/Microsoft.Extensions.DependencyInjection.dll",
"/_framework/_bin/Microsoft.Extensions.FileProviders.Abstractions.dll",
"/_framework/_bin/Microsoft.Extensions.Http.dll",
"/_framework/_bin/Microsoft.Extensions.Logging.Abstractions.dll",
"/_framework/_bin/Microsoft.Extensions.Logging.dll",
"/_framework/_bin/Microsoft.Extensions.Options.dll",
"/_framework/_bin/Microsoft.Extensions.Primitives.dll",
"/_framework/_bin/Microsoft.JSInterop.dll",
"/_framework/_bin/Mono.Security.dll",
"/_framework/_bin/Mono.WebAssembly.Interop.dll",
"/_framework/_bin/mscorlib.dll",
"/_framework/_bin/System.Buffers.dll",
"/_framework/_bin/System.ComponentModel.Annotations.dll",
"/_framework/_bin/System.Core.dll",
"/_framework/_bin/System.dll",
"/_framework/_bin/System.Memory.dll",
"/_framework/_bin/System.Net.Http.dll",
"/_framework/_bin/System.Numerics.Vectors.dll",
"/_framework/_bin/System.Runtime.CompilerServices.Unsafe.dll",
"/_framework/_bin/System.Text.Json.dll",
"/_framework/_bin/System.Threading.Tasks.Extensions.dll",
"/css/blazored.gitter.css",
"/css/blazored.gitter.min.css",
"/css/loader.css",
"/css/loader.min.css",
"/favicon.ico",
"/icon-192x192.png",
"/icon-512x512.png",
"/index.html",
"/ServiceWorkerRegister.js",
"/manifest.json"
];
// * listen for the install event and pre-cache anything in filesToCache * //
self.addEventListener(swInstallEvent, event => {
    self.skipWaiting();
    event.waitUntil(
        caches.open(staticCacheName)
            .then(cache => {
                return cache.addAll(requiredFiles);
            })
    );
});
self.addEventListener(swActivateEvent, function (event) {
    event.waitUntil(
        caches.keys().then(function (cacheNames) {
            return Promise.all(
                cacheNames.map(function (cacheName) {
                    if (staticCacheName !== cacheName && cacheName.startsWith(staticCachePrefix)) {
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});
self.addEventListener(networkFetchEvent, event => {
    const requestUrl = new URL(event.request.url);
    if (requestUrl.origin === location.origin) {
        if (requestUrl.pathname === baseURL) {
            event.respondWith(caches.match(indexURL));
            return;
        }
    }
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                if (response) {
                    return response;
                }
                return fetch(event.request)
                    .then(response => {
                        if (response.ok) {
                            if (requestUrl.origin === location.origin) {
                                caches.open(staticCacheName).then(cache => {
                                    cache.put(event.request.url, response);
                                });
                            }
                        }
                        return response.clone();
                    });
            }).catch(error => {
                console.error(error);
            })
    );
});
