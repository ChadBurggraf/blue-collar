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
        var stats;
        options = options || {};

        stats = new StatsModel({}, {jsonUrlRoot: this.jsonUrlRoot, navigateFragment: this.navigateFragment(), navigateUrlRoot: this.urlRoot});
        this.model.set({ChartsLoaded: !!options.chartsLoaded, Stats: stats}, {silent: true});

        stats.bind('counts', this.counts, this);
        this.view = new DashboardView({model: this.model});
    },

    /**
     * Performs an Ajax fetch on this instance's collection.
     */
    fetch: function() {
        var stats = this.model.get('Stats');
        this.model.set({Loading: true});

        stats.fetch({
            success: _.bind(function() {
                this.model.set({Loading: false});
            }, this, null),
            error: _.bind(this.error, this, null)
        });
    },

    /**
     * Renders the index view.
     */
    index: function() {
        this.page.html(this.view.render().el);
        this.fetch();
    },

    /**
     * Sets a value indicating whether the charts API has been loaded.
     *
     * @param {boolean} loaded A value indicating whether the charts API has been loaded.
     */
    setChartsLoaded: function(loaded) {
        this.model.set({ChartsLoaded: loaded});
    }
});