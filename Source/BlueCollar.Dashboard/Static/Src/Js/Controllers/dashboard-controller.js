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
        options = options || {};
        this.model = new StatsModel({ApplicationName: this.applicationName}, {jsonUrlRoot: this.jsonUrlRoot});
        this.model.bind('counts', this.counts, this);
        this.view = new DashboardView({model: this.model, chartsLoaded: options.chartsLoaded});
    },

    /**
     * Renders the index view.
     */
    index: function() {
        this.page.html(this.view.render().el);
        this.model.fetch({error: _.bind(this.error, this, null)});
    },

    /**
     * Sets a value indicating whether the charts API has been loaded.
     *
     * @param {boolean} loaded A value indicating whether the charts API has been loaded.
     */
    setChartsLoaded: function(loaded) {
        this.options.chartsLoaded = loaded;
        this.view.setChartsLoaded(loaded);
    }
});