/**
 * Dashboard area controller implementation.
 *
 * @constructor
 * @extends {CollarController}
 */
var DashboardController = CollarController.extend({
    /**
     * Initialization.
     *
     * @param {Object} options Initialization options.
     */
    initialize: function(options) {
        this.model = new StatsModel({ApplicationName: this.applicationName});
        this.model.urlRoot = this.urlRoot;
        this.model.bind('counts', this.counts, this);
        this.fetchOnIndex = true;

        if (options && options.stats) {
            this.model.set(this.model.parse(options.stats), {silent: true});
            this.fetchOnIndex = false;
        }

        this.view = new DashboardView({el: this.page, model: this.model});
    },

    index: function(options) {
        this.view.render();

        if (this.fetchOnIndex) {
            this.model.fetch({error: _.bind(this.ajaxError, this, null)});
        } else {
            this.fetchOnIndex = true;
        }
    }
});