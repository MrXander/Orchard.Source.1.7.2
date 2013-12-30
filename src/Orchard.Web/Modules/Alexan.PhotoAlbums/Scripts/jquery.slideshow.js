(function ($) {
    $.fn.slideshow = function (settings, navigation) {
        settings = jQuery.extend({
            thumbContainerSel: '#thumb-container',
            titleSel: '#slideshow-title',
            numberSel: '#slideshow-currentNumber',
            txtImage: 'Image', // (string) Specify text "Image"
            txtOf: 'of', 	// (string) Specify text "of"
            delayAnimation: 300, //miliseconds
            interval: 5, //seconds
            thumbSize: { width: 90, height: 90 },
            fullSize: { width: 640, height: 480 },
            autoplay: true
        }, settings);

        navigation = jQuery.extend({
            $prev: $('#slideshow-btn-prev'),
            $play: $('#slideshow-btn-play'),
            $next: $('#slideshow-btn-next')
        }, navigation);

        var slideshow = this;
        var imagesInfo = [];
        var currentIndex = -1;

        var $thumbContainer = $(settings.thumbContainerSel);
        var $title = $(settings.titleSel);
        var $numberText = $(settings.numberSel);
        var $image = $('#slideshow-image');

        function init() {
            imagesInfo = [];
            slideshow.find('li').addClass('thumbUnselected').each(function (index, element) {
                var $element = $(element);
                var $header = $element.find('h1').detach();
                var title = $header.find('a').text();
                var $thumb = $element.find('a.thumb');
                var $published = $element.find('.published').detach();
                imagesInfo.push({ $header: $header, title: title, $thumb: $thumb, $published: $published, src: $thumb.attr('href') });
            });

            var $thumbs = $('a.thumb', $thumbContainer);
            $thumbs.click(function (event) {
                var index = $thumbs.index($(this));
                setImage(index);
                event.preventDefault();
            });
        }

        function initNavigation() {
            settings.autoplay ? navigation.$play.addClass('slideshow-btn-stop') : navigation.$play.addClass('slideshow-btn-play');

            navigation.$prev.click(function (event) {
                prev();
                event.preventDefault();
            });

            navigation.$next.click(function (event) {
                next();
                event.preventDefault();
            });

            navigation.$play.click(function (event) {
                if (playObj) {
                    stop();
                    navigation.$play.removeClass('slideshow-btn-stop').addClass('slideshow-btn-play');
                }
                else {
                    play();
                    navigation.$play.removeClass('slideshow-btn-play').addClass('slideshow-btn-stop');
                }
                event.preventDefault();
            });

        }

        function selectThumb() {
            $li = slideshow.find('li').removeClass('thumbSelected');
            var $currentThumb = $($li.get(currentIndex)).addClass('thumbSelected');

            if (currentIndex == 0) {
                $thumbContainer.scrollLeft(0);
            }
            if (currentIndex == (imagesInfo.length - 1)) {
                $thumbContainer.scrollLeft($thumbContainer.find('ul').width());
            }
            else if ($currentThumb.position().left > $thumbContainer.width()) {
                $thumbContainer.scrollLeft($thumbContainer.scrollLeft() + $currentThumb.offset().left - $thumbContainer.width() / 3);
            }
            else if (($currentThumb.position().left - $currentThumb.outerWidth()) < 0) {
                $thumbContainer.scrollLeft($thumbContainer.scrollLeft() + $currentThumb.offset().left - $thumbContainer.width() / 1.5);
            }
        }

        function setTitle(title, index) {
            $title.text(title);
            $title.fadeIn(settings.delayAnimation);
            $numberText.text(settings.txtImage + ' ' + (index + 1) + ' ' + settings.txtOf + ' ' + imagesInfo.length);
        }

        function setImage(index) {
            if (index == currentIndex)
                return;
            clearTimeout(playObj);
            currentIndex = index;

            var img = imagesInfo[index];
            if (!img) {
                return;
            }

            $image.fadeOut(settings.delayAnimation, function () {
                $image.attr('src', img.src);
                $image.attr('alt', img.title);
            });
            $title.fadeOut(settings.delayAnimation);

            $image.unbind().load(function () {
                var offset = $image.parent().height() - $image.height();
                $image.css('top', offset);
                $image.fadeIn(settings.delayAnimation);
                setTitle(img.title, index);
                selectThumb();
                if (autoPlay) {
                    playObj = setTimeout(function () {
                        next();
                    }, settings.interval * 1000);
                }
            });
        }

        function preloadImages() {
            // Preload the image
            $.each(imagesInfo, function (index, item) {
                var imagePreload = new Image();
                imagePreload.src = item.src;
            });
        }

        // Safe way to get the next image index relative to the current image.
        // If the current image is the last, returns 0
        function getNextIndex(index) {
            var nextIndex = index + 1;
            if (nextIndex >= imagesInfo.length)
                nextIndex = 0;
            return nextIndex;
        }

        // Safe way to get the previous image index relative to the current image.
        // If the current image is the first, return the index of the last image in the gallery.
        function getPrevIndex(index) {
            var prevIndex = index - 1;
            if (prevIndex < 0)
                prevIndex = imagesInfo.length - 1;
            return prevIndex;
        }

        function next() {
            var index = getNextIndex(currentIndex);
            setImage(index);
        }

        function prev() {
            var index = getPrevIndex(currentIndex);
            setImage(index);
        }

        var autoPlay = settings.autoplay;
        playObj = undefined;
        function play() {
            autoPlay = true;
            next();
        }

        function stop() {
            autoPlay = false;
            if (playObj) {
                clearTimeout(playObj);
            }
        }

        init();
        setImage(0);
        //preload
        preloadImages();
        initNavigation();
    }

})(jQuery);