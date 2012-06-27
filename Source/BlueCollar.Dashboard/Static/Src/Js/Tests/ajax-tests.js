module('Ajax');

asyncTest('NavCollectionFetch', function() {
    var list = new NavCollection();
    list.url = jsonUrlRoot + 'counts';
    
    list.fetch({
        success: function(collection, response) {
            equal(list.length, 6);
            start();
        },
        error: fetchError
    });
});