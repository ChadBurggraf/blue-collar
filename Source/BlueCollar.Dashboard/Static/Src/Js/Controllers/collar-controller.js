/**
 * Base controller implementation.
 *
 * @constructor
 * @param {jQuery} page A reference to the page jQuery element.
 * @param {Object} options Initialization options. 
 */
var CollarController = function(page, options) {
    this.page = page;
    this.options = _.extend({}, options);
    this.initialize(this.options);
};

/**
 * Static functions.
 */
_.extend(CollarController, {
    /**
     * Mixin extend functionality to enable inheritence.
     */
    extend: extend
});

/**
 * Prototype functions.
 */
_.extend(CollarController.prototype, Backbone.Events, {
    collection: null,
    fragment: '',
    listView: null,

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {},

    /**
     * Generic handler for Ajax-related errors.
     *
     * @param {Backbone.View} view The view the error is related to.
     * @param {Backbone.Model} model The model the error is related to.
     * @param {jqXHR} response The response received from the server.
     */
    ajaxError: function(view, model, response) {
        if (view && _.isFunction(view.hideLoading)) {
            view.hideLoading();
        }

        if (this.collection) {
            this.collection.each(function(m) { 
                m.set({editing: false}); 
            });
        }

        if (!view || !view.ajaxError(model, response)) {
            var message;

            switch (response.status) {
                case 400:
                    message = 'Bad Request (400): The server indicated that you submitted was invalid or impropertly formatted.';
                    break;
                case 403:
                    message = 'Forbidden (403): You are not authorized to access the requested resource.';
                    break;
                case 404:
                    message = 'Not Found (404): The requested resource was not found on the server.';
                    break;
                case 500:
                    message = 'Internal Server Error (500): Something when wrong when processing your request on the server.';
                    break;
                default:
                    message = 'Unknown Error (' + response.status + '): An unknown error occurred while processing your request on the server.';
                    break;
            }

            NoticeView.create({
                className: 'error',
                model: {Title: 'Uh Oh, That Kinda Hurt', Message: message}
            });
        }
    }
});