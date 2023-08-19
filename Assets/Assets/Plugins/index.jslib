mergeInto(LibraryManager.library, {
  GetUrlParam: function(param) {
    param = UTF8ToString(param); // Convert C string to JS string
    let searchParams = new URLSearchParams(window.location.search);
    let result = searchParams.get(param) || "";
    // Convert JS string to C string
    let bufferSize = lengthBytesUTF8(result) + 1; 
    let buffer = _malloc(bufferSize);
    stringToUTF8(result, buffer, bufferSize);
    return buffer;
  }
});