/**
 * Base controller implementation for major "page" areas of the application.
 * Provides error handling, a default index action and default submit
 * and destroy behavior for models.
 * 
 * @constructor
 * @this {BlueCollarController}
 * @param {Dashboard} router A Dashboard instance.
 * @param {string} urlRoot The JSON URL root the application is using.
 * @param {jQuery} main A jQuery reference to the #main element on the page.
 * @param {jQuery} forms A jQuery reference to the #forms element on the page.
 * @param {object} options Initialization options.
 */
var BlueCollarController = function(router, urlRoot, main, forms, options) {
    this.router = router;
    this.urlRoot = urlRoot;
    this.main = main.html('');
    this.forms = forms.html('');
    this.options = _.extend({}, options);

    if (_.isFunction(this.collection)) {
        this.collection = new this.collection(null, {urlRoot: this.urlRoot});
        this.collection.bind('reset', this.reset, this);
    }

    this.initialize(this.options);
};

// BlueCollarController static functions.
_.extend(BlueCollarController, {
    extend: extend
});

// BlueCollarController prototype (instnace functions).
_.extend(BlueCollarController.prototype, Backbone.Events, {
    collection: BlueCollarCollection,
    fragment: '',
    listView: null,

    initialize: function() {},

    add: function(listView) {},

    ajaxError: function(view, model, response) {
        if (view) {
            view.hideLoading();
        }

        this.collection.each(function(m) { m.set({editing: false}); });

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
    },

    counts: function(listView, counts) {
        this.trigger('counts', this, counts);
    },

    createListView: function() {
        var view = new this.listView({collection: this.collection});
        view.bind('search', this.search, this);
        view.bind('page', this.pageChange, this);
        view.bind('add', this.add, this);
        view.bind('edit', this.edit, this);
        view.bind('counts', this.counts, this);
        return view;
    },

    destroy: function(view, successMessage) {
        var fetch = _.bind(this.fetch, this),
            collection = this.collection,
            that = this;
        
        view.showLoading();

        view.model.destroy({
            success: function(model, response) {
                collection.clearEditing();

                fetch();
                view.remove();

                NoticeView.create({
                    className: 'success',
                    model: {
                        Title: 'Success!', 
                        Message: successMessage
                    }
                });
            },
            error: _.bind(this.ajaxError, this, view)
        });
    },

    edit: function(listView, model) {},

    fetch: function() {
        this.collection.fetch({error: _.bind(this.ajaxError, this, null)});
    },

    index: function(options) {
        options = options || {};

        this.main.html(this.createListView().render({loading: true}).el);
        this.forms.html('');

        this.collection.search = options.search || '';
        this.collection.pageNumber = options.page || 1;
        this.fetch();
    },

    navigate: function() {
        var fragment = this.navigateFragment(),
            search = this.collection.search || '',
            page = this.collection.pageNumber || 1;

        if (this.collection && (search || page > 1)) {
            fragment += '/' + encodeURIComponent(search) + '/p' + encodeURIComponent(page.toString());
        }

        this.router.navigate(fragment, true);
    },

    navigateFragment: function() {
        return this.fragment || '';
    },

    pageChange: function(listView, page) {
        this.collection.pageNumber = page;
        this.navigate();
    },

    reset: function() {
        if (this.collection.length === 0 && this.collection.pageNumber > 1) {
            this.collection.pageNumber = this.collection.pageNumber - 1;
            this.navigate();
        }
    },

    search: function(listView, search) {
        this.collection.pageNumber = 1;
        this.collection.search = search;
        this.navigate();
    },

    submit: function(view, attributes, successMessage) {
        var fetch = _.bind(this.fetch, this),
            collection = this.collection,
            isNew = _.isUndefined(attributes.Id) || attributes.Id < 1,
            that = this;
        
        view.showLoading();
        
        view.model.save(attributes, {
            success: function(model, response) {
                collection.clearEditing();

                fetch();
                view.remove();

                NoticeView.create({
                    className: 'success',
                    model: {
                        Title: 'Success!', 
                        Message: successMessage
                    }
                });
            },
            error: _.bind(this.ajaxError, this, view)
        });
    }
});