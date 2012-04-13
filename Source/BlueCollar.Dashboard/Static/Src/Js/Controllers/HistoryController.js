/**
 * Controller for History management.
 */
var HistoryController = BlueCollarController.extend({
    collection: HistoryList,
    fragment: 'history',
    listView: HistoryListView,

    createListView: function() {
        var view = BlueCollarController.prototype.createListView.call(this);
        view.bind('display', this.display, this);
        return view;
    },

    display: function(listView, model) {
        var view;

        if (!model.details) {
            model.details = new HistoryDetails(model.attributes);
            model.details.owner = model;
            model.details.urlRoot = this.urlRoot;
            model.details.fetch({error: _.bind(this.ajaxError, this)});
        }

        view = new HistoryDetailsView({model: model.details});

        this.collection.clearEditing();
        model.set({editing: true});

        this.forms.html(view.render().el);
        view.bind('submit', this.done, this);

        scrollTo(0, 0);
    },

    done: function(view, attributes) {
        this.collection.clearEditing();
        view.remove();
    }
});