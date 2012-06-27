function fetchError(collection, response) {
    ok(false, 'Error handler was called with status code ' + response.status + '.');
    start();
}