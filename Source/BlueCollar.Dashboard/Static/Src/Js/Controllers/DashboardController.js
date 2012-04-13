/**
 * Controller for the dashboard stats.
 */
var DashboardController = BlueCollarController.extend({
    collection: null,
    fragment: '',

    initialize: function(options) {
        this.stats = new Stats();
        this.stats.urlRoot = this.urlRoot;
        this.stats.bind('counts', this.counts, this);
        this.fetchOnIndex = true;
        
        if (options && options.stats) {
            this.stats.set(this.stats.parse(options.stats), {silent: true});
            this.fetchOnIndex = false;
        }
    },

    index: function(options) {
        var view = new DashboardView({model: this.stats});
        this.main.html(view.el);
        this.forms.html('');

        // Google Visualization requires that the target elements actually
        // be in the DOM, apparently. That's why we're rendering here
        // instead of inline while writing the HTML to the page element.
        view.render();

        if (this.fetchOnIndex) {
            this.stats.fetch({error: _.bind(this.ajaxError, this, null)});
        } else {
            this.fetchOnIndex = true;
        }
    }
});