/**
 * Scheduled jobs area controller implementation.
 *
 * @constructor
 * @extends {CollarController}
 */
var ScheduledJobsController = CollarController.extend({
    collection: ScheduledJobCollection,
    fragment: 'schedules',

    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.model.bind('change:ScheduleId', this.scheduleIdChange, this);
        this.model.set({ScheduleName: '', Fragment: this.fragment}, {silent: true});
        this.view = new ScheduledJobsView({model: this.model});
        this.view.bind('fetch', this.fetch, this);
        this.view.bind('editDelete', this.editDelete, this);
        this.view.bind('editSubmit', this.editSubmit, this);
        this.view.bind('orderSubmit', this.orderSubmit, this);
    },

    /**
     * Renders the index view.
     *
     * @param {Number} id The requested schedule ID.
     * @param {String} search The search string to filter the view on.
     * @param {Number} page The page number to filter the view on.
     * @param {Number} jid The requested record ID to display.
     * @param {String} action The requested record action to take.
     */
    index: function(id, search, page, jid, action) {
        if (id && !_.isNumber(id)) {
            id = parseInt(id, 10);

            if (id < 0 || isNaN(id)) {
                id = 0;
            }
        } else {
            id = 0;
        }

        if (page && !_.isNumber(page)) {
            page = parseInt(page, 10);
        } else {
            page = 1;
        }

        if (jid && !_.isNumber(jid)) {
            jid = parseInt(jid, 10);

            if (jid < 0 || isNaN(jid)) {
                jid = 0;
            }
        } else {
            jid = 0;
        }

        this.model.set({ScheduleId: id});
        this.model.set({Search: search || '', PageNumber: page, Id: jid, Action: action || '', Loading: true}, {silent: true});
        this.view.delegateEvents();
        this.page.html(this.view.render().el);
        this.fetch();
    },

    /**
     * Gets the URL fragment to use when navigating.
     *
     * @return {String} A URL fragment.
     */
    navigateFragment: function() {
        var fragment = this.fragment,
            scheduleId = this.model ? this.model.get('ScheduleId') : 0;

        if (scheduleId) {
            fragment = fragment.appendUrlPath('id').appendUrlPath(scheduleId).appendUrlPath('jobs');
        }

        return fragment;
    },

    /**
     * Handles the view's orderSubmit event.
     *
     * @param {Object} sender The event sender.
     * @param {Object} args The event arguments.
     */
    orderSubmit: function(sender, args) {
        var model = new ScheduledJobsOrderModel({}, {scheduleId: this.model.get('ScheduleId')});

        model.save(args.Attributes, {
            success: _.bind(function(args, model, response) {
                this.success(args, model, response);
                this.fetch();
            }, this, args),
            error: _.bind(this.error, this, args)
        });
    },

    /**
     * Handles this instance's model's schedule ID change event.
     */
    scheduleIdChange: function() {
        this.getCollection().setScheduleId(this.model.get('ScheduleId'));
    }
});