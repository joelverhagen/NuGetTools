// Write your Javascript code.
$("#version-select").change(function (event) {
    var url = $(this).val();
    if (url) {
        document.location = url;
    }
});