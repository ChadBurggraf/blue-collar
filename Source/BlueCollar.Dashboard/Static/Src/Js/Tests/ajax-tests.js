module('Ajax');

asyncTest('HistoryCollectionFetch', function() {
    var coll = new HistoryCollection([], {jsonUrlRoot: jsonUrlRoot});

    coll.fetch({
        success: fetchSuccess,
        error: fetchError
    });
});

asyncTest('NavCollectionFetch', function() {
    var coll = new NavCollection([], {jsonUrlRoot: jsonUrlRoot});
    
    coll.fetch({
        success: function(collection, response) {
            equal(collection.length, 6);
            start();
        },
        error: fetchError
    });
});

asyncTest('QueueCollectionFetch', function() {
    var coll = new QueueCollection([], {jsonUrlRoot: jsonUrlRoot});

    coll.fetch({
        success: fetchSuccess,
        error: fetchError
    });
});

asyncTest('ScheduleCollectionFetch', function() {
    var coll = new ScheduleCollection([], {jsonUrlRoot: jsonUrlRoot});

    coll.fetch({
        success: fetchSuccess,
        error: fetchError
    });
});

asyncTest('WorkerCollectionFetch', function() {
    var coll = new WorkerCollection([], {jsonUrlRoot: jsonUrlRoot});

    coll.fetch({
        success: fetchSuccess,
        error: fetchError
    });
});

asyncTest('WorkingCollectionFetch', function() {
    var coll = new WorkingCollection([], {jsonUrlRoot: jsonUrlRoot});

    coll.fetch({
        success: fetchSuccess,
        error: fetchError
    });
});

function fetchError(collection, response) {
    ok(false, 'Error handler was called with status code ' + response.status + '.');
    start();
}

function fetchSuccess(collection, response) {
    expect(0);
    start();
}