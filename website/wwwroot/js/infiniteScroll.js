window.infiniteScroll = {
    registerObserver: function (sentinel, dotNetHelper) {
        const options = {
            root: null,
            rootMargin: '0px',
            threshold: 0.1
        };
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    dotNetHelper.invokeMethodAsync("LoadMore");
                }
            });
        }, options);
        observer.observe(sentinel);
    }
};