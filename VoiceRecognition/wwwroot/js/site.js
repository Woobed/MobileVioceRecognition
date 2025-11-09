// wwwroot/js/site.js
window.playActivationSound = function () {
    try {
        const a = document.getElementById('activateAudio');
        if (!a) return;
        a.currentTime = 0;
        const p = a.play();
        if (p !== undefined) {
            p.catch(err => console.warn("play() rejected:", err));
        }
    } catch (e) {
        console.error(e);
    }
}
