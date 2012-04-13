module('Ajax');

asyncTest('NavigationItemListFetch', function() {
    var list = new NavigationItemList();
    list.url = jsonUrlRoot + 'counts';
    
    list.fetch({
        success: function(collection, response) {
            equal(list.length, 6);
            start();
        },
        error: fetchError
    });
});