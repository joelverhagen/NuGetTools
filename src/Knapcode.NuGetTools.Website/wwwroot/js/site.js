// Change the NuGet version when selection changes.
$("#version-select").change(function (event) {
    var url = $(this).val();
    if (url) {
        document.location = url;
    }
});

// Enable autocomplete for framework short folder names.
var frameworksShortFolderNames = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.whitespace,
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    prefetch: '../api/frameworks/short-folder-names'
});

frameworksShortFolderNames.initialize();

$(".framework-short-folder-name-typeahead").typeahead({
    autoSelect: false,
    source: frameworksShortFolderNames.ttAdapter()
});

// Enable autocomplete for framework identifiers.
var frameworksIdentifiers = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.whitespace,
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    prefetch: '../api/frameworks/identifiers'
});

frameworksIdentifiers.initialize();

$(".framework-identifier-typeahead").typeahead({
    autoSelect: false,
    source: frameworksIdentifiers.ttAdapter()
});
