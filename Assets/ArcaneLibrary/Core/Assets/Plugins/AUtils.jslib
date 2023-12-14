mergeInto(LibraryManager.library, {

  IsIframe: function() { return !!((window !== window.top) | 0); },

});