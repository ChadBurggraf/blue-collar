/**
 * Manages the root scheduled jobs view.
 *
 * @constructor
 * @extends {AreaView}
 */
var ScheduledJobsView = AreaView.extend({
    template: _.template($('#scheduled-jobs-template').html()),

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        AreaView.prototype.initialize.call(this, options);
        this.model.bind('change:ScheduleName', this.renderScheduleName, this);
        this.listView = new ScheduledJobsListView({model: this.model});
        this.listView.bind('edit', this.edit, this);

        this.events = _.extend({}, this.events, {
            'click .table-actions .btn-order': 'order',
            'keypress .table-form input[name="Number"]': 'keypressNumber'
        });

        this.delegateEvents();
    },

    /**
     * Handle's the add button's click event.
     */
    add: function() {
        var model = new ScheduledJobModel({}, {jsonUrlRoot: this.model.jsonUrlRoot, scheduleId: this.model.get('ScheduleId')});
        this.model.clearId();
        this.renderIdView($('.details'), model);
    },

    /**
     * Handles a number input's keypress event.
     *
     * @param DOMEvent event The event that was fired.
     */
    keypressNumber: function(event) {
        if ((event.keyCode || event.which) === 13) {
            this.orderSubmit();
        }
    },

    /**
     * Handles the order button's click event.
     */
    order: function() {
        this.orderSubmit();
    },

    /**
     * Submits the table form for an order update if changes are detected.
     */
    orderSubmit: function() {
        var numbers = this.serializeOrder();

        if (numbers.length > 0) {
            this.trigger('orderSubmit', this, {Attributes: {Numbers: numbers}, Action: 'ordered'});
        }
    },

    /**
     * Renders the ID view for the given model in the given details element.
     *
     * @param {jQuery} el The jQuery object containing the details element to render into.
     * @param {CollarModel} model The model to render the ID view for.
     */
    renderIdView: function(el, model) {
        var view = new ScheduledJobsEditView({model: model});
        view.bind('cancel', this.editCancel, this);
        view.bind('delete', this.editDelete, this);
        view.bind('submit', this.editSubmit, this);
        el.html(view.render().el);
        view.focus();
    },

    /**
     * Renders the current schedule's name in its container.
     */
    renderScheduleName: function() {
        this.$('.page-header h4 a').text(this.model.get('ScheduleName'));
        return this;
    },

    /**
     * Serializes changes in the list's order values.
     *
     * @return Array A set of changes in the list's order values.
     */
    serializeOrder: function() {
        var inputs = this.$('.table-form input[name="Number"]'),
            changes = {},
            numbers = [],
            count = 0,
            input,
            curr,
            orig,
            prop,
            i,
            n;

        for (i = 0, n = inputs.length; i < n; i++) {
            input = $(inputs[i]);
            curr = parseInt(input.val(), 10);
            orig = parseInt(input.data('original-value'), 10);

            if (!isNaN(curr) && curr > 0 && curr !== orig) {
                changes[input.data('job-id')] = curr;
                count++;
            }
        }
            
        if (count > 0) {
            for (prop in changes) {
                if (changes.hasOwnProperty(prop)) {
                    numbers.push({Id: prop, Number: changes[prop]});
                }
            }
        }

        return numbers;
    }
});