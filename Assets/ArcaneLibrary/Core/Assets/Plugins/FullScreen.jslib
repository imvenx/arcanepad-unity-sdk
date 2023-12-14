mergeInto(LibraryManager.library, {
  SetFullScreen: function() {
    var container = document.querySelector('#unity-container');
    var canvas = document.querySelector('#unity-canvas');
    var unityFooter = document.querySelector('#unity-footer');
    var body = document.body;

    if (unityFooter) {
        unityFooter.style.display = 'none';
    }

    if(container){
        container.style.width = '100vw';
        container.style.height = '100vh';
        container.classList.remove('unity-desktop');
    }

    canvas.style.width = '100vw';
    canvas.style.height = '100vh';
    body.style.margin = '0';
    body.style.overflow = 'hidden'; 

    window.addEventListener('resize', function() {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    });
  },
});
