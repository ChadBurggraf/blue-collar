(function($) {
	var $win = $(window), 
		$nav = $('.subnav'), 
		navTop = $('.subnav').length && $('.subnav').offset().top, 
		isFixed = false;

    processScroll();

    $nav.on('click', function () {
    	if (!isFixed) {
    		setTimeout(function () {  
                $win.scrollTop($win.scrollTop() - 60); 
            }, 10);
    	}
    })

    $win.on('scroll', processScroll);
    $win.on('resize', processResize);

    function processResize() {
    	var $navParent;

    	if (isFixed) {
    		$navParent = $nav.parent();

    		if ($navParent.length > 0) {
    			$nav.css('left', $navParent.offset().left + 'px');
    		}
    	}
    }

    function processScroll() {
		var scrollTop = $win.scrollTop();

		if (scrollTop >= navTop && !isFixed) {
			isFixed = true;
			$nav.addClass('subnav-fixed');
		} else if (scrollTop <= navTop && isFixed) {
			isFixed = false;
			$nav.removeClass('subnav-fixed');
			$nav.css('left', '');
		}

		processResize();
    }
})(jQuery);