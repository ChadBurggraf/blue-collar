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
            'click .page-header h4 a': 'up'
        });

        this.delegateEvents();
    },

    /**
     * Handle's the add button's click event.
     *
    add: function() {
        var model = new ScheduleModel({StartOn: Date.today()});
        model.urlRoot = this.model.get('UrlRoot');
        this.model.clearId();
        this.renderIdView($('.details'), model);
    },

    /**
     * Renders the ID view for the given model in the given details element.
     *
     * @param {jQuery} el The jQuery object containing the details element to render into.
     * @param {CollarModel} model The model to render the ID view for.
     *
    renderIdView: function(el, model) {
        var view = new SchedulesEditView({model: model});
        view.bind('cancel', this.editCancel, this);
        view.bind('delete', this.editDelete, this);
        view.bind('submit', this.editSubmit, this);
        el.html(view.render().el);
        view.focus();
    }*/

    renderScheduleName: function() {
        this.$('.page-header h4 a').text(this.model.get('ScheduleName'));
        return this;
    },

    up: function() {
        this.model.set({ScheduleId: 0});
    }
});