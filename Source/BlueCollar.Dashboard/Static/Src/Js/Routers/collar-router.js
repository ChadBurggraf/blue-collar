/**
 * Base router implementation.
 *
 * @constructor
 */
var CollarRouter = Backbone.Router.extend({
    /**
     * Initialization.
     *
     * @param {App} app The root application object.
     * @param {Object} options Initialization options.
     */
    initialize: function(app, options) {
        this.app = app;
        this.options = _.extend({}, options);
    },

    /**
     * Handles controller-initiated navigate events.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    controllerNavigate: function(sender, args) {
        var url;

        args = _.extend({
            Action: '',
            Fragment: '',
            Id: 0,
            Options: {},
            PageNumber: 1,
            Search: ''
        }, args);

        url = args.Fragment;

        if (args.Search) {
            url += '/q/' + encodeURIComponent(args.Search.toString());
        }

        if (args.PageNumber > 1) {
            url += '/p/' + encodeURIComponent(args.PageNumber.toString());
        }

        if (args.Id > 0) {
            url += '/id/' + encodeURIComponent(args.Id.toString());
            
            if (args.Action) {
                url += '/' + encodeURIComponent(args.Action.toString());
            }
        }

        this.navigate(url, args);
    },

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
     * Creates a new instance of this router's default controller.
     *
     * @param {Function} func The constructor function of the controller to create.
     * @param {Object} options Initialization options to use when creating the controller.
     * @return The created controller.
     */
    createController: function(func, options) {
        var controller = new func(
            this.app.name,
            this.app.urlRoot,
            this.app.jsonUrlRoot,
            this.app.page,
            options);

        controller.bind('counts', this.counts, this);
        controller.bind('navigate', this.controllerNavigate, this);
        return controller;
    },

    /**
     * Handles the ID route.
     *
     * @param {Number} id The requested record ID.
     */
    id: function(id) {
        this.index('', 1, id, '');
    },

    /**
     * Handles the ID + action route.
     *
     * @param {Number} id The requested record ID.
     * @param {String} action The requested record action.
     */
    idAction: function(id, action) {
        this.index('', 1, id, action);
    },

    /**
     * Handles the index route.
     *
     * @param {String} search The requested search string.
     * @param {Number} page The requested page number.
     * @param {Number} id The requested record ID.
     * @param {String} action The requested record action.
     */
    index: function(search, page, id, action) {
        this.controller.index(
            decodeURIComponent((search || '').toString()), 
            decodeURIComponent((page || '1').toString()), 
            decodeURIComponent((id || '').toString()),
            decodeURIComponent((action || '').toString()));

        this.trigger('nav', this, {name: this.name});
    },

    /**
     * Handles the paging route.
     *
     * @param {Number} page The requested page number.
     */
    page: function(page) {
        this.index('', page, '', '');
    },

    /**
     * Handles paging + ID route.
     *
     * @param {Number} search The requested page number.
     * @param {Number} id The requested record ID.
     */
    pageId: function(page, id) {
        this.index('', page, id, '');
    },

    /**
     * Handles paging + ID + action route.
     *
     * @param {Number} search The requested page number.
     * @param {Number} id The requested record ID.
     * @param {String} action The requested record action.
     */
    pageIdAction: function(page, id, action) {
        this.index('', page, id, action);
    },

    /**
     * Handles the search route.
     *
     * @param {String} search The requested search string.
     */
    search: function(search) {
        this.index(search, 1, '', '');
    },

    /**
     * Handles the search + ID route.
     *
     * @param {String} search The requested search string.
     * @param {Number} id The requested record ID.
     */
    searchId: function(search, id) {
        this.index(search, 1, id, '');
    },

    /**
     * Handles the search + ID + action route.
     *
     * @param {String} search The requested search string.
     * @param {Number} id The requested record ID.
     * @param {Action} action The requested record action.
     */
    searchIdAction: function(search, id, action) {
        this.index(search, 1, id, action);
    },

    /**
     * Handles the search + paging route.
     *
     * @param {String} search The requested search string.
     * @param {Number} page The requested page number.
     */
    searchPage: function(search, page) {
        this.index(search, page, '', '');
    }
});