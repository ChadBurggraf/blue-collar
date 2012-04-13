/**
 * Details view for History models.
 */
var HistoryDetailsView = FormView.extend({
    template: _.template($('#history-details-template').html()),

    initialize: function(options) {
        FormView.prototype.initialize.call(this, options);
        this.model.bind('change', this.render, this);
    },

    render: function() {
        var ex = this.model.get('Exception'),
            exField,
            message,
            frames,
            exP,
            exCode;

        FormView.prototype.render.call(this);
        exField = this.$('.clearfix.exception');
        
        if (ex) {
            ex = $($.parseXML(ex));
            message = ex.find('Message').text();
            frames = ex.find('Frame');
            exP = exField.find('p');
            exCode = exField.find('code');
            
            if (message) {
                exP.text(message).show();
            }

            if (frames.length > 0) {
                exCode.text(_.map(frames, function(f) { return $(f).text(); }).join('\n'));
            }

            if (message && frames.length > 0) {
                exField.show();
            } else {
                exField.hide();
            }
        } else {
            exField.hide();
        }

        setTimeout(prettyPrint, 100);
        return this;
    }
});