const serviceWorkerFileName = '/ServiceWorker.js';
const swInstalledEvent = 'installed';
const staticCachePrefix = 'blazored-gitter-v';
const updateAlertMessage = 'Update available. Reload the page when convenient.';
window.updateAvailable = new Promise(function (resolve, reject) {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register(serviceWorkerFileName)
            .then(function (registration) {
                console.log('Registration successful, scope is:', registration.scope);
                registration.onupdatefound = () => {
                    const installingWorker = registration.installing;
                    installingWorker.onstatechange = () => {
                        switch (installingWorker.state) {
                            case swInstalledEvent:
                                if (navigator.serviceWorker.controller) {
                                    resolve(true);
                                } else {
                                    resolve(false);
                                }
                                break;
                            default:
                        }
                    };
                };
            })
            .catch(error =>
                console.log('Service worker registration failed, error:', error));
    }
});

window.addEventListener('beforeinstallprompt', function (e) {
    // Prevent Chrome 67 and earlier from automatically showing the prompt
    e.preventDefault();
    // Stash the event so it can be triggered later.
    window.PWADeferredPrompt = e;

    showAddToHomeScreen();

});
window['updateAvailable']
    .then(isAvailable => {
        if (isAvailable) {
            alert(updateAlertMessage);
        }
    });
function showAddToHomeScreen() {
    var pwaInstallPrompt = document.createElement('div');
    var pwaInstallButton = document.createElement('button');
    var pwaCancelButton = document.createElement('button');

    pwaInstallPrompt.id = 'pwa-install-prompt';
    pwaInstallPrompt.style.position = 'absolute';
    pwaInstallPrompt.style.bottom = '1rem';
    pwaInstallPrompt.style.left = '1rem';
    pwaInstallPrompt.style.right = '1rem';
    pwaInstallPrompt.style.padding = '0.3rem';
    pwaInstallPrompt.style.display = 'flex';
    pwaInstallPrompt.style.backgroundColor = 'lightslategray';
    pwaInstallPrompt.style.color = 'white';
    pwaInstallPrompt.style.fontFamily = 'sans-serif';
    pwaInstallPrompt.style.fontSize = '1.2rem';
    pwaInstallPrompt.style.borderRadius = '4px';

    pwaInstallButton.style.marginLeft = 'auto';
    pwaInstallButton.style.width = '4em';
    pwaInstallButton.style.backgroundColor = '#00796B';
    pwaInstallButton.style.color = 'white';
    pwaInstallButton.style.border = 'none';
    pwaInstallButton.style.borderRadius = '25px';

    pwaCancelButton.style.marginLeft = '0.3rem';
    pwaCancelButton.style.width = '4em';
    pwaCancelButton.style.backgroundColor = '#9d0d0d';
    pwaCancelButton.style.color = 'white';
    pwaCancelButton.style.border = 'none';
    pwaCancelButton.style.borderRadius = '25px';

    pwaInstallPrompt.innerText = 'Add to your homescreen?';
    pwaInstallButton.innerText = 'ok';
    pwaCancelButton.innerText = 'no';

    pwaInstallPrompt.appendChild(pwaInstallButton);
    pwaInstallPrompt.appendChild(pwaCancelButton);
    document.body.appendChild(pwaInstallPrompt);

    pwaInstallButton.addEventListener('click', addToHomeScreen);
    pwaCancelButton.addEventListener('click', hideAddToHomeScreen);
    setTimeout(hideAddToHomeScreen, 10000);
}

function hideAddToHomeScreen() {
    var pwa = document.getElementById('pwa-install-prompt');
    if (pwa) document.body.removeChild(pwa);
}

function addToHomeScreen(s, e) {
    hideAddToHomeScreen();
    if (window.PWADeferredPrompt) {
        window.PWADeferredPrompt.prompt();
        window.PWADeferredPrompt.userChoice
            .then(function (choiceResult) {
                window.PWADeferredPrompt = null;
            });
    }
}
