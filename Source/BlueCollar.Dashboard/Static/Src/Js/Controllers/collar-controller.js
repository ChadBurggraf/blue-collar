/**
 * Base controller implementation.
 *
 * @constructor
 * @param {String} applicationName The name of the application.
 * @param {String} urlRoot The navigate URL root the application is using.
 * @param {String} jsonUrlRoot The JSON URL root the application is using.
 * @param {jQuery} page A reference to the page jQuery element.
 * @param {Object} options Initialization options. 
 */
var CollarController = function(applicationName, urlRoot, jsonUrlRoot, page, options) {
    var collection;

    this.applicationName = applicationName;
    this.urlRoot = urlRoot;
    this.jsonUrlRoot = jsonUrlRoot;
    this.page = page;
    this.options = _.extend({}, options);

    collection = new this.collection([], {jsonUrlRoot: this.jsonUrlRoot, navigateFragment: this.navigateFragment(), navigateUrlRoot: this.urlRoot});
    collection.bind('counts', this.counts, this);

    this.model = new AreaModel({ApplicationName: this.applicationName, Collection: collection}, {jsonUrlRoot: this.jsonUrlRoot, navigateFragment: this.navigateFragment(), navigateUrlRoot: this.urlRoot});
    this.model.bind('change:Id', this.navigate, this);
    this.model.bind('change:Action', this.navigate, this);
    
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
    collection: CollarCollection,
    fragment: '',

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {},

    /**
     * Handles counts update events.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    counts: function(sender, args) {
        this.trigger('counts', this, args);
    },

    /**
     * Handle's this instance's view's details event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    details: function(sender, args) {
        args.Model.fetch({
            success: function() {
                args.Model.set({DetailsLoaded: true}, {silent: true});
            },
            error: _.bind(this.error, this)
        });
    },

    /**
     * Handle's this instance's view's editDelete event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    editDelete: function(sender, args) {
        args.Model.destroy({
            success: _.bind(this.success, this, args),
            error: _.bind(this.error, this, args)
        });
    },

    /**
     * Handle's this instance's view's editSubmit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    editSubmit: function(sender, args) {
        args.Model.save(args.Attributes, {
            success: _.bind(this.success, this, args),
            error: _.bind(this.error, this, args),
            wait: true
        });
    },

    /**
     * Handles an error response from the server.
     *
     * @param {Object} args The original event arguments that initiated the server action.
     * @param {CollarModel} model The model that the server action was taken on behalf of.
     * @param {jqXHR} response The response received from the server.
     */
    error: function(args, model, response) {
        var handled = false,
            message;

        if (args && args.View) {
            args.View.hideLoading();

            if (args.View.error(response)) {
                handled = true;
            } else {
                args.View.remove();
            }
        }

        if (this.model.get('Loading')) {
            this.model.set({Loading: false});
        }
        
        if (!handled) {
            if (_.isFunction(this.model.clearId)) {
                this.model.clearId();
            }

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
                className: 'alert-error',
                model: {Title: 'Uh Oh, That Kinda Hurt', Message: message}
            });
        }
    },

    /**
     * Performs an Ajax fetch on this instance's collection.
     */
    fetch: function() {
        var collection = this.getCollection();

        if (collection) {
            this.model.set({Loading: true});

            collection.fetch({
                pageNumber: this.model.get('PageNumber'),
                search: this.model.get('Search'),
                error: _.bind(this.error, this, null)
            });
            
            this.navigate();
        }
    },

    /**
     * Gets this instance's current collection instance, if applicable.
     *
     * @return {CollarCollection} The current collection instance, or null if none exists.
     */
    getCollection: function() {
        var collection = this.model && _.isFunction(this.model.get) ? this.model.get('Collection') : null;
        return collection && !_.isUndefined(collection.length) ? collection : null;
    },

    /**
     * Renders the index view.
     *
     * @param {String} search The search string to filter the view on.
     * @param {Number} page The page number to filter the view on.
     * @param {Number} id The requested record ID to display.
     * @param {String} action The requested record action to take.
     */
    index: function(search, page, id, action) {
        if (page && !_.isNumber(page)) {
            page = parseInt(page, 10);
        } else {
            page = 1;
        }

        if (id && !_.isNumber(id)) {
            id = parseInt(id, 10);

            if (id < 0 || isNaN(id)) {
                id = 0;
            }
        } else {
            id = 0;
        }
        
        this.model.set({Search: search || '', PageNumber: page, Id: id, Action: action || '', Loading: true}, {silent: true});
        this.view.delegateEvents();
        this.page.html(this.view.render().el);
        this.fetch();
    },

    /**
     * Performs navigation on behalf of this controller.
     */
    navigate: function() {
        this.trigger('navigate', this, _.extend(this.model.toJSON(), {Fragment: this.navigateFragment()}));
    },

    /**
     * Gets the URL fragment to use when navigating.
     *
     * @return {String} A URL fragment.
     */
    navigateFragment: function() {
        return this.fragment || '';
    },

    /**
     * Handle's this instance's view's signalSubmit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    signalSubmit: function(sender, args) {
        var collection = this.getCollection(),
            model;

        if (collection) {
            model = collection.find(function(m) { return m.get('Id') === args.Model.get('Id'); });
        
            if (model) {
                model.set({Signal: args.Attributes.Signal});
            }
        }

        args.Model.save(args.Attributes, {
            success: _.bind(this.success, this, args),
            error: _.bind(this.error, this, args),
            wait: true
        });
    },

    /**
     * Handles a success response from the server.
     *
     * @param {Object} args The original event arguments that initiated the server action.
     * @param {CollarModel} model The model that the server action was taken on behalf of.
     * @param {jqXHR} response The response received from the server.
     */
    success: function(args, model, response) {
        if (_.isFunction(this.model.clearId)) {
            this.model.clearId();
        }

        if (args.View) {
            args.View.remove();
        }

        if (args.Action === 'created' || args.Action === 'deleted') {
            this.fetch();
        }
    }
});