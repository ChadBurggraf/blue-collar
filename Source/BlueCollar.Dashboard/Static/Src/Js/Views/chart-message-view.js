var ChartMessageView = Backbone.View.extend({
    className: 'chart-message',
    tagName: 'div',
    template: _.template($('#chart-message-template').html()),

    render: function() {
        this.$el.html(this.template(this.model.toJSON()));
        return this;
    }
});