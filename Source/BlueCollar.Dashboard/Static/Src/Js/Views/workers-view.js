/**
 * Manages the root workers view.
 *
 * @constructor
 * @extends {AreaView}
 */
var WorkersView = AreaView.extend({
    template: _.template($('#workers-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        AreaView.prototype.initialize.call(this, options);

        this.listView = new WorkersListView({model: this.model});
        this.listView.bind('edit', this.edit, this);

        this.editView = new WorkersFormView({model: this.model});
    },

    /**
     * Handles the list view's edit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    edit: function(sender, args) {
        this.model.get('Collection').clearSelected({silent: true});
        args.Model.set({Selected: true}, {silent: true});
        this.model.set({Editing: args.Model});
    }
});