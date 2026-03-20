const CACHE_NAME = 'rezeptbuch-cache-v1';

const urlsToCache = [
    './',
    './app.css',
    './bootstrap/bootstrap.min.css',
    './RecipeApp.styles.css',
    './icon-192.png',
    './icon-512.png',
    './favicon.png',
    './manifest.json',
    './_framework/blazor.web.js'
];

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                return cache.addAll(urlsToCache);
            })
    );
});

self.addEventListener('fetch', event => {
    // Basic network-first strategy for Blazor Server
    event.respondWith(
        fetch(event.request)
            .catch(() => {
                return caches.match(event.request);
            })
    );
});

self.addEventListener('activate', event => {
    const cacheWhitelist = [CACHE_NAME];
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cacheName => {
                    if (cacheWhitelist.indexOf(cacheName) === -1) {
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});
