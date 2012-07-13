/**
 * Provides a static collection of simple execution queues for reducing
 * a series of rapidly-fired function calls into a single call.
 *
 * This queue is suitable only when every function call in the succession
 * is inter-changeable with every other call. Namespace groups of equivalent
 * functions into their own execution queues.
 *
 * Example: TimeoutQueue.enqueue('prettyPrint', prettyPrint);
 *
 * The above call will enqueue an execution of the global prettyPrint()
 * function on the 'prettyPrint' queue. If no more executions are added
 * to the queue after 300ms, the last instance of the function passed to
 * the queue will be executed.
 */
var TimeoutQueue = {
    _queues: {},

    /**
     * Enqueues a function execution on the queue with the specified name.
     *
     * @param {String} name The name of the queue to enqueue the exeuction onto.
     * @param {Function} func The function call to enqueue.
     * @param {Object} options A set of options to override defaults with.
     */
    enqueue: function(name, func, options) {
        var q = TimeoutQueue._queues[name],
            number;

        options = _.extend({
            timeout: 300
        }, options);

        if (!q) {
            q = {items: [], func: func};
            TimeoutQueue._queues[name] = q;
        } else {
            q.func = func;
        }

        number = q.items.length + 1;
        q.items.push(number);
        setTimeout(_.bind(TimeoutQueue._dequeue, TimeoutQueue, name, number), options.timeout);
    },

    _dequeue: function(name, number) {
        var q = TimeoutQueue._queues[name],
            length = 0,
            func;
            
        if (q) {
            length = q.items.length;

            if (q.items.length === number) {
                func = q.func;
                q.items = [];
                delete q.func;
            }
        }

        if (_.isFunction(func)) {
            func();
        }
    },
};