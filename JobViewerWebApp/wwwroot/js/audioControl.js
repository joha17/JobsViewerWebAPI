
window.audioControl = {
    muteTab: function () {
        // Obtener todos los elementos de audio/video de la página
        let mediaElements = document.querySelectorAll('audio, video');

        // Silenciar cada uno de los elementos
        mediaElements.forEach(el => {
            el.muted = true;
        });
    },
    unmuteTab: function () {
        // Obtener todos los elementos de audio/video de la página
        let mediaElements = document.querySelectorAll('audio, video');

        // Reactivar el sonido en cada uno de los elementos
        mediaElements.forEach(el => {
            el.muted = false;
        });
    }
};