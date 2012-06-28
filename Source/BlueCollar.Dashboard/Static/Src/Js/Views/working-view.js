/**
 * Manages the root working jobs view.
 *
 * @constructor
 * @extends {AreaView}
 */
var WorkingView = AreaView.extend({
    template: _.template($('#working-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        AreaView.prototype.initialize.call(this, options);
        this.listView = new WorkingListView({model: this.model});
        this.listView.bind('edit', this.edit, this);
    },

    /**
     * Handles the list view's edit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    edit: function(sender, args) {
        debugger;
        var editView = new WorkersEditView({model: args.Model});
        this.$('.details').html(editView.render().el);
    }
});