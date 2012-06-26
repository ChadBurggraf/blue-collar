/**
 * Manages the row view for the queue list.
 *
 * @constructor
 * @extends {RowView}
 */
var QueueRowView = RowView.extend({
    template: _.template($('#queue-row-template').html()),

    /**
     * Renders the view.
     *
     * @return {RowView} This instance.
     */
    render: function() {
        var status,
            css;

        RowView.prototype.render.call(this);

        /*status = this.model.get('Status');

        switch (status) {
            case 'Succeeded':
                css = 'green';
                break;
            case 'Failed':
            case 'TimedOut':
                css = 'red';
                break;
            default:
                css = '';
                break;
        }

        this.$('.status').removeClass('red green').addClass(css);*/
        return this;
    }
});