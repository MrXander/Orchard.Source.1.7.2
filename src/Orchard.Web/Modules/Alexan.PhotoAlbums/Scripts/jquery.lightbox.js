(function ($) {
    $.fn.lightBox = function (settings) {
        // Settings to configure the jQuery lightBox plugin how you like
        settings = jQuery.extend({
            overlayOpacity: 0.8, 	// (integer) Opacity value to overlay; inform: 0.X. Where X are number from 0 to 9
            //fixedNavigation: true, 	// (boolean) Boolean that informs if the navigation (next and prev button) will be fixed or not in the interface.
            containerBorderSize: 10, 		// (integer) If you adjust the padding in the CSS for the container, #lightbox-container-image-box, you will need to update this value
            delayAnimation: 400, 	// (integer) Specify the resize duration of container image. These number are miliseconds. 400 is default.
            txtImage: 'Image', // (string) Specify text "Image"
            txtOf: 'of', 	// (string) Specify text "of"
            // Configuration related to keyboard navigation
            keyToClose: 'c', 	// (string) (c = close) Letter to close the jQuery lightBox interface. Beyond this letter, the letter X and the SCAPE key is used to.
            keyToPrev: 'p', 	// (string) (p = previous) Letter to show the previous image
            keyToNext: 'n', 	// (string) (n = next) Letter to show the next image.
            // Don´t alter these variables in any way
            imageArray: [],
            activeImage: 0
        }, settings);
        // Caching the jQuery object with all elements matched
        var jQueryMatchedObj = this; // This, in this context, refer to jQuery object
        /**
        * Initializing the plugin calling the start function
        *
        * @return boolean false
        */
        function initialize() {
            start(this, jQueryMatchedObj); // This, in this context, refer to object (link) which the user have clicked
            return false; // Avoid the browser following the link
        }
        /**
        * Start the jQuery lightBox plugin
        *
        * @param object objClicked The object (link) whick the user have clicked
        * @param object jQueryMatchedObj The jQuery object with all elements matched
        */
        function start(objClicked, jQueryMatchedObj) {
            // Hime some elements to avoid conflict with overlay in IE. These elements appear above the overlay.
            $('embed, object, select').css({ 'visibility': 'hidden' });
            // Call the function to create the markup structure; style some elements; assign events in some elements.
            setInterface();
            // Unset total images in imageArray
            settings.imageArray.length = 0;
            // Unset image active information
            settings.activeImage = 0;
            // We have an image set? Or just an image? Let´s see it.

            // Add an Array (as many as we have), with href and title atributes, inside the Array that storage the images references		
            for (var i = 0; i < jQueryMatchedObj.length; i++) {
                settings.imageArray.push({
                    href: $(jQueryMatchedObj[i]).attr('href'),
                    title: $('img', jQueryMatchedObj[i]).attr('title'),
                    date: $('.published', jQueryMatchedObj[i]).text()
                });
            }

            //set current image
            while (settings.imageArray[settings.activeImage].href != objClicked.getAttribute('href')) {
                settings.activeImage++;
            }
            // Call the function that prepares image exibition
            setImage();
        }

        function setInterface() {
            // Get page sizes
            var pageSize = getPageSize();
            // Style overlay and show   it
            $('#jquery-overlay').css({
                opacity: settings.overlayOpacity
            }).fadeIn();
            resizeOverlay();
            // Get page scroll
            var pageScroll = getPageScroll();
            // Calculate top and left offset for the jquery-lightbox div object and show it
            //var top = (pageSize.windowHeight - jQuery('#jquery-lightbox').height()) / 2;
            var top = 10;
            $('#jquery-lightbox').css({
                top: pageScroll.yScroll + top,
                left: pageScroll.xScroll
            }).show();
            // Assigning click events in elements to close overlay
            $('#jquery-overlay, #lightbox-secNav-btnClose').click(function () {
                finish();
                return false;
            });
            // If window was resized, calculate the new overlay dimensions
            $(window).resize(function () {
                // Get page sizes
                var pageSize = getPageSize();
                resizeOverlay();
                // Get page scroll
                var pageScroll = getPageScroll();
                // Calculate top and left offset for the jquery-lightbox div object and show it
                var top = (pageSize.windowHeight - jQuery('#jquery-lightbox').height()) / 2;
                if (top < 10) {
                    top = 10;
                }
                $('#jquery-lightbox').css({
                    top: pageScroll.yScroll + top,
                    left: pageScroll.xScroll
                });
            });

            //$('#lightbox-header, #lightbox-footer').width($('#lightbox-container-image-box').width());
        }
        /**
        * Prepares image exibition; doing a image´s preloader to calculate it´s size
        *
        */
        function setImage() { // show the loading
            // Show the loading
            $('#lightbox-loading').show();
            $('#lightbox-image,#lightbox-container-image-data-box').hide();

            // Image preload process
            var objImagePreloader = new Image();
            objImagePreloader.onload = function () {
                $('#lightbox-image').attr('src', settings.imageArray[settings.activeImage].href);
                // Perfomance an effect in the image container resizing it
                resizeContainer(objImagePreloader.width, objImagePreloader.height);
                //	clear onLoad, IE behaves irratically with animated gifs otherwise
                objImagePreloader.onload = function () { };
            };
            var href = settings.imageArray[settings.activeImage].href;
            objImagePreloader.src = href;
        };

        function resizeOverlay() {
            var pageSize = getPageSize();
            $('#jquery-overlay').css({
                width: pageSize.pageWidth,
                height: pageSize.pageHeight
            });
        }
        /**
        * Perfomance an effect in the image container resizing it
        *
        * @param integer intImageWidth The image´s width that will be showed
        * @param integer intImageHeight The image´s height that will be showed
        */
        function resizeContainer(intImageWidth, intImageHeight) {
            // Get current width and height
            var intCurrentWidth = $('#lightbox-container-image-box').width();
            var intCurrentHeight = $('#lightbox-container-image-box').height();
            // Get the width and height of the selected image plus the padding
            var intWidth = (intImageWidth + (settings.containerBorderSize * 2)); // Plus the image´s width and the left and right padding value
            var intHeight = (intImageHeight + (settings.containerBorderSize * 2)); // Plus the image´s height and the left and right padding value
            // Diferences
            var intDiffW = intCurrentWidth - intWidth;
            var intDiffH = intCurrentHeight - intHeight;
            // Perfomance the effect
            $('#lightbox-container-image-box').animate({ width: intWidth, height: intHeight }, settings.delayAnimation, function () {
                showImage();
                resizeOverlay();
            });

            $('#lightbox-container-image-data-box').css({ width: intImageWidth });

            $('#lightbox-nav-btnPrev,#lightbox-nav-btnNext').css({ height: intImageHeight + (settings.containerBorderSize * 2) });
        };
        /**
        * Show the prepared image
        *
        */
        function showImage() {
            $('#lightbox-loading').hide();
            $('#lightbox-image').fadeIn(settings.delayAnimation, function () {
                showHeaderFooter();
                setNavigation();
            });
            preloadImages();
            $('#lightbox-overley').css('height', '100%');
        };
        /**
        * Show the image information
        *
        */
        function showHeaderFooter() {
            $('#lightbox-container-image-data-box').slideDown(settings.delayAnimation);

            var title = settings.imageArray[settings.activeImage].title;
            if (title) {
                $('#lightbox-image-details-caption').html(title).show();
            }
            // If we have a image set, display 'Image X of X'
            if (settings.imageArray.length > 1) {
                $('#lightbox-image-details-currentNumber').html(settings.txtImage + ' ' + (settings.activeImage + 1) + ' ' + settings.txtOf + ' ' + settings.imageArray.length).show();
            }

        }
        /**
        * Display the button navigations
        *
        */
        function setNavigation() {
            var $nav = $('#lightbox-nav').show();

            // Show the prev button, if not the first image in set
            var $btnPrev = $('#lightbox-nav-btnPrev');
            if (settings.activeImage != 0) {
                $btnPrev.show().unbind('click').bind('click', function () {
                    settings.activeImage = settings.activeImage - 1;
                    setImage();
                    return false;
                });
            }
            else {
                $btnPrev.hide();
            }

            // Show the next button, if not the last image in set
            var $btnNext = $('#lightbox-nav-btnNext')
            if (settings.activeImage != (settings.imageArray.length - 1)) {
                $btnNext.show().unbind('click').bind('click', function () {
                    settings.activeImage = settings.activeImage + 1;
                    setImage();
                    return false;
                });
            }
            else {
                $btnNext.hide();
            }
            // Enable keyboard navigation
            enableKeyboardNavigation();
        }
        /**
        * Enable a support to keyboard navigation
        *
        */
        function enableKeyboardNavigation() {
            $(document).keydown(function (objEvent) {
                keyboardAction(objEvent);
            });
        }
        /**
        * Disable the support to keyboard navigation
        *
        */
        function disableKeyboardNavigation() {
            $(document).unbind();
        }
        /**
        * Perform the keyboard actions
        *
        */
        function keyboardAction(objEvent) {
            // To ie
            if (objEvent == null) {
                keycode = event.keyCode;
                escapeKey = 27;
                // To Mozilla
            } else {
                keycode = objEvent.keyCode;
                escapeKey = objEvent.DOM_VK_ESCAPE;
            }
            // Get the key in lower case form
            key = String.fromCharCode(keycode).toLowerCase();
            // Verify the keys to close the ligthBox
            if ((key == settings.keyToClose) || (key == 'x') || (keycode == escapeKey)) {
                finish();
            }
            // Verify the key to show the previous image
            if ((key == settings.keyToPrev) || (keycode == 37)) {
                // If we´re not showing the first image, call the previous
                if (settings.activeImage != 0) {
                    settings.activeImage = settings.activeImage - 1;
                    setImage();
                    disableKeyboardNavigation();
                }
            }
            // Verify the key to show the next image
            if ((key == settings.keyToNext) || (keycode == 39)) {
                // If we´re not showing the last image, call the next
                if (settings.activeImage != (settings.imageArray.length - 1)) {
                    settings.activeImage = settings.activeImage + 1;
                    setImage();
                    disableKeyboardNavigation();
                }
            }
        }
        /**
        * Preload prev and next images being showed
        *
        */
        function preloadImages() {
            if ((settings.imageArray.length - 1) > settings.activeImage) {
                objNext = new Image();
                objNext.src = settings.imageArray[settings.activeImage + 1].href;
            }
            if (settings.activeImage > 0) {
                objPrev = new Image();
                objPrev.src = settings.imageArray[settings.activeImage - 1].href;
            }
        }
        /**
        * Remove jQuery lightBox plugin HTML markup
        *
        */
        function finish() {
            $('#jquery-lightbox').hide();
            $('#jquery-overlay').fadeOut(function () { $('#jquery-overlay').hide(); });
            // Show some elements to avoid conflict with overlay in IE. These elements appear above the overlay.
            $('embed, object, select').css({ 'visibility': 'visible' });
        }
        /**
        / THIRD FUNCTION
        * getPageSize() by quirksmode.com
        *
        * @return Array Return an array with page width, height and window width, height
        */
        function getPageSize() {
            var xScroll, yScroll;
            if (window.innerHeight && window.scrollMaxY) {
                xScroll = window.innerWidth + window.scrollMaxX;
                yScroll = window.innerHeight + window.scrollMaxY;
            } else if (document.body.scrollHeight > document.body.offsetHeight) { // all but Explorer Mac
                xScroll = document.body.scrollWidth;
                yScroll = document.body.scrollHeight;
            } else { // Explorer Mac...would also work in Explorer 6 Strict, Mozilla and Safari
                xScroll = document.body.offsetWidth;
                yScroll = document.body.offsetHeight;
            }
            var windowWidth, windowHeight;
            if (self.innerHeight) {	// all except Explorer
                if (document.documentElement.clientWidth) {
                    windowWidth = document.documentElement.clientWidth;
                } else {
                    windowWidth = self.innerWidth;
                }
                windowHeight = self.innerHeight;
            } else if (document.documentElement && document.documentElement.clientHeight) { // Explorer 6 Strict Mode
                windowWidth = document.documentElement.clientWidth;
                windowHeight = document.documentElement.clientHeight;
            } else if (document.body) { // other Explorers
                windowWidth = document.body.clientWidth;
                windowHeight = document.body.clientHeight;
            }
            // for small pages with total height less then height of the viewport
            if (yScroll < windowHeight) {
                pageHeight = windowHeight;
            } else {
                pageHeight = yScroll;
            }
            // for small pages with total width less then width of the viewport
            if (xScroll < windowWidth) {
                pageWidth = xScroll;
            } else {
                pageWidth = windowWidth;
            }

            return {
                pageWidth: pageWidth,
                pageHeight: pageHeight,
                windowWidth: windowWidth,
                windowHeight: windowHeight
            };
        };
        /**
        / THIRD FUNCTION
        * getPageScroll() by quirksmode.com
        *
        * @return Array Return an array with x,y page scroll values.
        */
        function getPageScroll() {
            var xScroll, yScroll;
            if (self.pageYOffset) {
                yScroll = self.pageYOffset;
                xScroll = self.pageXOffset;
            } else if (document.documentElement && document.documentElement.scrollTop) {	 // Explorer 6 Strict
                yScroll = document.documentElement.scrollTop;
                xScroll = document.documentElement.scrollLeft;
            } else if (document.body) {// all other Explorers
                yScroll = document.body.scrollTop;
                xScroll = document.body.scrollLeft;
            }
            return { xScroll: xScroll, yScroll: yScroll };
        };

        $('#lightbox-nav-btnPrev, #lightbox-nav-btnNext')
            .css({ opacity: 0 })
            .hover(function () {
                $(this).css({ opacity: 1 });
            }, function () {
                $(this).css({ opacity: 0 });
            });
        // Return the jQuery object for chaining. The unbind method is used to avoid click conflict when the plugin is called more than once
        return this.unbind('click').click(initialize);
    };
})(jQuery);                          // Call and execute the function immediately passing the jQuery object