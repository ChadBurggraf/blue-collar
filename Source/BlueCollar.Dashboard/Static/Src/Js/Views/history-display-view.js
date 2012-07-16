/**
 * Implements the history display form.
 *
 * @constructor
 * @extends {FormView}
 */
var HistoryDisplayView = FormView.extend({
    template: _.template($('#history-display-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        FormView.prototype.initialize.call(this, options);

        this.events = _.extend({}, this.events, {
            'click button.btn-primary': 'enqueue'    
        });

        this.delegateEvents();
    },

    /**
     * Handles model change events.
     */
    change: function() {
        this.render();
    },

    enqueue: function() {
        this.trigger('enqueue', this, {Model: this.model});
    },

    /**
     * Renders the view.
     *
     * @return {FormView} This instance.
     */
    render: function() {
        FormView.prototype.render.call(this);

        if (!this.model.get('Exception')) {
            this.$('.exception').remove();
        }

        TimeoutQueue.enqueue('prettyPrint', prettyPrint);
        return this;
    }
});