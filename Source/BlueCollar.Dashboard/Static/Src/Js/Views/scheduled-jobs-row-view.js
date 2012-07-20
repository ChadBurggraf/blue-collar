/**
 * Manages the row view for the scheduled jobs list.
 *
 * @constructor
 * @extends {RowView}
 */
var ScheduledJobsRowView = RowView.extend({
    template: _.template($('#scheduled-jobs-row-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        RowView.prototype.initialize.call(this, options);

        this.events = _.extend({}, this.events, {
            'change td input[name="Number"]': 'changeNumber'
        });

        this.delegateEvents();
    },

    /**
     * Handles a number input's change event.
     */
    changeNumber: function() {
        var inputs = this.$('input[name="Number"]'),
            changed = false,
            input,
            curr,
            orig,
            i,
            n;
        
        for (i = 0, n = inputs.length; i < n; i++) {
            input = $(inputs[i]);
            curr = parseInt(input.val(), 10);
            orig = parseInt(input.data('original-value'), 10);

            if (!isNaN(curr) && curr > 0 && curr !== orig) {
                this.trigger('number', this, {Model: this.model, Number: curr, Original: orig});
                break;
            }
        }
    },

    /**
     * Renders the view.
     *
     * @return {RowView} This instance.
     */
    render: function() {
        RowView.prototype.render.call(this);
        TimeoutQueue.enqueue('prettyPrint', prettyPrint);
        return this;
    }
});