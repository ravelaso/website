// 🌐 COMPLETE BLAZOR SSR HERO ANIMATION SCRIPT
// Place this ENTIRE script in App.razor <head> section

(function() {
    'use strict';

    // 🔧 Global namespace to prevent conflicts
    window.HeroAnimation = window.HeroAnimation || {};

    console.log('🚀 Loading Blazor SSR Hero Animation...');

    // 🎛️ PHYSICS CONFIGURATION
    const CONFIG = {
        INITIAL_VELOCITY: 0.03,
        MAX_SPEED: 0.08,
        FRICTION: 0.895,
        RETURN_FORCE: 0.003,
        BOUNDARY_PUSH: 0.005,
        COLLISION_RADIUS: 3,
        COLLISION_FORCE: 0.02,
        MOUSE_DISTANCE: 10,
        MOUSE_REPEL_FORCE: 0.05,
        GLOW_MAX_SIZE: 40,
        SCALE_MAX: 0.2,
        TARGET_FPS: 60,
        BOUNDARY_MARGIN: 5,
        ICON_COUNT: {
            mobile: 8,
            tablet: 15,
            desktop: 35,
            large: 50
        }
    };

    // 📏 Device detection
    function getDeviceType() {
        const width = window.innerWidth;
        if (width < 640) return 'mobile';
        if (width < 1024) return 'tablet';
        if (width < 1440) return 'desktop';
        return 'large';
    }

    function getIconCount() {
        return CONFIG.ICON_COUNT[getDeviceType()];
    }

    // 🎯 Main initialization function
    function initializeAnimation() {
        console.log('🎬 Attempting to initialize hero animation...');

        // 🛡️ Wait for DOM to be ready
        const heroSection = document.getElementById('hero-section');
        const floatingElements = document.querySelectorAll('.floating-element');

        if (!heroSection || floatingElements.length === 0) {
            console.log('⏳ DOM not ready, retrying in 50ms...');
            setTimeout(initializeAnimation, 50);
            return;
        }

        // 🛡️ Prevent multiple initializations on same page
        if (window.HeroAnimation.currentPageInitialized) {
            console.log('✅ Animation already running on this page');
            return;
        }

        console.log('✅ DOM ready, starting animation setup...');

        // 🧹 Clean up any existing animation
        if (window.HeroAnimation.cleanup) {
            window.HeroAnimation.cleanup();
        }

        // 📱 Responsive icon management
        const targetIconCount = getIconCount();
        const visibleElements = Array.from(floatingElements).slice(0, targetIconCount);

        // Hide excess elements
        floatingElements.forEach((el, index) => {
            el.style.display = index < targetIconCount ? '' : 'none';
        });

        console.log(`🎯 Device: ${getDeviceType()}, Showing ${visibleElements.length} icons`);

        // 💾 Element data cache
        const elementsData = visibleElements.map((element, index) => ({
            element,
            id: index,
            x: parseFloat(element.style.left) || Math.random() * 90 + 5,
            y: parseFloat(element.style.top) || Math.random() * 90 + 5,
            vx: (Math.random() - 0.5) * CONFIG.INITIAL_VELOCITY,
            vy: (Math.random() - 0.5) * CONFIG.INITIAL_VELOCITY,
            radius: CONFIG.COLLISION_RADIUS,
            originalX: parseFloat(element.style.left) || Math.random() * 90 + 5,
            originalY: parseFloat(element.style.top) || Math.random() * 90 + 5
        }));

        // Set initial positions if not set
        elementsData.forEach(data => {
            data.element.style.left = `${data.x}%`;
            data.element.style.top = `${data.y}%`;
        });

        // 🎮 Animation state
        let animationId = null;
        let lastTime = 0;
        const frameTime = 1000 / CONFIG.TARGET_FPS;
        let mouseX = 0, mouseY = 0, isMouseMoving = false;

        // 📱 Mobile optimizations
        const deviceType = getDeviceType();
        if (deviceType === 'mobile') {
            CONFIG.INITIAL_VELOCITY *= 0.5;
            CONFIG.MAX_SPEED *= 0.5;
            CONFIG.MOUSE_REPEL_FORCE *= 0.5;
            CONFIG.TARGET_FPS = 30;
        }

        // 🎬 Animation functions
        function updatePositions() {
            elementsData.forEach(data => {
                const dx = data.originalX - data.x;
                const dy = data.originalY - data.y;

                data.vx += dx * CONFIG.RETURN_FORCE;
                data.vy += dy * CONFIG.RETURN_FORCE;
                data.vx *= CONFIG.FRICTION;
                data.vy *= CONFIG.FRICTION;

                data.x += data.vx;
                data.y += data.vy;

                // Boundary constraints
                if (data.x < CONFIG.BOUNDARY_MARGIN) {
                    data.vx += (CONFIG.BOUNDARY_MARGIN - data.x) * CONFIG.BOUNDARY_PUSH;
                }
                if (data.x > 95 - CONFIG.BOUNDARY_MARGIN) {
                    data.vx -= (data.x - (95 - CONFIG.BOUNDARY_MARGIN)) * CONFIG.BOUNDARY_PUSH;
                }
                if (data.y < CONFIG.BOUNDARY_MARGIN) {
                    data.vy += (CONFIG.BOUNDARY_MARGIN - data.y) * CONFIG.BOUNDARY_PUSH;
                }
                if (data.y > 95 - CONFIG.BOUNDARY_MARGIN) {
                    data.vy -= (data.y - (95 - CONFIG.BOUNDARY_MARGIN)) * CONFIG.BOUNDARY_PUSH;
                }

                data.element.style.left = `${data.x}%`;
                data.element.style.top = `${data.y}%`;
            });
        }

        function checkCollisions() {
            for (let i = 0; i < elementsData.length; i++) {
                for (let j = i + 1; j < elementsData.length; j++) {
                    const a = elementsData[i];
                    const b = elementsData[j];
                    const dx = a.x - b.x;
                    const dy = a.y - b.y;
                    const distance = Math.sqrt(dx * dx + dy * dy);

                    if (distance < a.radius) {
                        const angle = Math.atan2(dy, dx);
                        a.vx += Math.cos(angle) * CONFIG.COLLISION_FORCE;
                        a.vy += Math.sin(angle) * CONFIG.COLLISION_FORCE;
                        b.vx -= Math.cos(angle) * CONFIG.COLLISION_FORCE;
                        b.vy -= Math.sin(angle) * CONFIG.COLLISION_FORCE;

                        // Limit velocity
                        a.vx = Math.max(-CONFIG.MAX_SPEED, Math.min(CONFIG.MAX_SPEED, a.vx));
                        a.vy = Math.max(-CONFIG.MAX_SPEED, Math.min(CONFIG.MAX_SPEED, a.vy));
                        b.vx = Math.max(-CONFIG.MAX_SPEED, Math.min(CONFIG.MAX_SPEED, b.vx));
                        b.vy = Math.max(-CONFIG.MAX_SPEED, Math.min(CONFIG.MAX_SPEED, b.vy));
                    }
                }
            }
        }

        function animate(currentTime) {
            if (currentTime - lastTime >= frameTime) {
                updatePositions();
                checkCollisions();
                lastTime = currentTime;
            }
            animationId = requestAnimationFrame(animate);
        }

        function updateMouseEffects() {
            if (!isMouseMoving) return;

            elementsData.forEach(data => {
                const dx = mouseX - data.x;
                const dy = mouseY - data.y;
                const distance = Math.sqrt(dx * dx + dy * dy);

                if (distance < CONFIG.MOUSE_DISTANCE) {
                    const intensity = Math.max(0, (CONFIG.MOUSE_DISTANCE - distance) / CONFIG.MOUSE_DISTANCE);
                    const glowSize = Math.round(intensity * CONFIG.GLOW_MAX_SIZE);
                    const opacity = intensity * 2;
                    const brightness = 1 + intensity * 2;
                    const scale = 1 + intensity * CONFIG.SCALE_MAX;

                    data.element.style.filter = `
                        drop-shadow(0 0 ${glowSize}px rgba(0, 150, 255, ${opacity}))
                        drop-shadow(0 0 ${glowSize * 1.5}px rgba(100, 200, 255, ${opacity * 0.8}))
                        brightness(${brightness})
                    `;
                    data.element.style.transform = `scale(${scale})`;
                    data.element.style.transition = 'none';

                    const angle = Math.atan2(data.y - mouseY, data.x - mouseX);
                    data.vx += Math.cos(angle) * CONFIG.MOUSE_REPEL_FORCE * intensity;
                    data.vy += Math.sin(angle) * CONFIG.MOUSE_REPEL_FORCE * intensity;
                } else {
                    data.element.style.filter = '';
                    data.element.style.transform = '';
                    data.element.style.transition = 'filter 0.15s ease-out, transform 0.15s ease-out';
                }
            });

            isMouseMoving = false;
        }

        // 🖱️ Event handlers
        function handleMouseMove(e) {
            const rect = heroSection.getBoundingClientRect();
            mouseX = ((e.clientX - rect.left) / rect.width) * 100;
            mouseY = ((e.clientY - rect.top) / rect.height) * 100;
            isMouseMoving = true;
            requestAnimationFrame(updateMouseEffects);
        }

        function handleMouseLeave() {
            elementsData.forEach(data => {
                data.element.style.filter = '';
                data.element.style.transform = '';
                data.element.style.transition = 'filter 0.2s ease-out, transform 0.2s ease-out';
            });
        }

        let resizeTimeout;
        function handleResize() {
            clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(() => {
                const newIconCount = getIconCount();
                if (newIconCount !== visibleElements.length) {
                    console.log('🔄 Reinitializing due to resize...');
                    window.HeroAnimation.currentPageInitialized = false;
                    initializeAnimation();
                }
            }, 250);
        }

        // 🎯 Add event listeners
        heroSection.addEventListener('mousemove', handleMouseMove);
        heroSection.addEventListener('mouseleave', handleMouseLeave);
        window.addEventListener('resize', handleResize);

        // 🧹 Cleanup function
        function cleanup() {
            console.log('🧹 Cleaning up hero animation...');

            if (animationId) {
                cancelAnimationFrame(animationId);
                animationId = null;
            }

            heroSection?.removeEventListener('mousemove', handleMouseMove);
            heroSection?.removeEventListener('mouseleave', handleMouseLeave);
            window.removeEventListener('resize', handleResize);

            elementsData.forEach(data => {
                if (data.element) {
                    data.element.style.filter = '';
                    data.element.style.transform = '';
                    data.element.style.transition = '';
                }
            });

            window.HeroAnimation.currentPageInitialized = false;
        }

        // 🎬 Start animation
        animate(0);

        // 💾 Store cleanup function globally and mark as initialized
        window.HeroAnimation.cleanup = cleanup;
        window.HeroAnimation.currentPageInitialized = true;

        console.log('✅ Hero animation started successfully!');
    }

    // 🎯 PAGE NAVIGATION DETECTION FUNCTION
    function checkForHeroSection() {
        if (document.getElementById('hero-section')) {
            console.log('🏠 Hero section detected, initializing animation...');
            initializeAnimation();
        } else {
            console.log('📄 No hero section on this page');
            // Clean up if we're on a different page
            if (window.HeroAnimation.cleanup) {
                window.HeroAnimation.cleanup();
            }
        }
    }

    // 🚀 BLAZOR SSR INITIALIZATION STRATEGIES

    // Strategy 1: Initial page load
    if (document.readyState === 'complete') {
        checkForHeroSection();
    } else if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', checkForHeroSection);
    } else {
        checkForHeroSection();
    }

    // Strategy 2: Enhanced navigation (Blazor SSR navigation)
    document.addEventListener('enhancedload', function() {
        console.log('🔄 Enhanced navigation detected');
        setTimeout(checkForHeroSection, 50);
    });

    // Strategy 3: Page show event (handles browser back/forward)
    window.addEventListener('pageshow', function(event) {
        console.log('📄 Page show event detected');
        setTimeout(checkForHeroSection, 50);
    });

    // Strategy 4: Mutation observer for dynamic content
    const observer = new MutationObserver(function(mutations) {
        mutations.forEach(function(mutation) {
            if (mutation.type === 'childList') {
                const heroSection = document.getElementById('hero-section');
                if (heroSection && !window.HeroAnimation.currentPageInitialized) {
                    console.log('🔍 Hero section appeared via mutation observer');
                    setTimeout(checkForHeroSection, 50);
                }
            }
        });
    });

    // Start observing when DOM is ready
    if (document.body) {
        observer.observe(document.body, { childList: true, subtree: true });
    } else {
        document.addEventListener('DOMContentLoaded', function() {
            observer.observe(document.body, { childList: true, subtree: true });
        });
    }

    // Strategy 5: Periodic check (fallback)
    let periodicCheck = setInterval(function() {
        if (document.getElementById('hero-section') && !window.HeroAnimation.currentPageInitialized) {
            console.log('⏰ Periodic check found hero section');
            checkForHeroSection();
        }
    }, 500);

    // Stop periodic check after 10 seconds
    setTimeout(function() {
        clearInterval(periodicCheck);
    }, 10000);

    // 🧹 Cleanup on page unload
    window.addEventListener('beforeunload', function() {
        if (window.HeroAnimation?.cleanup) {
            window.HeroAnimation.cleanup();
        }
        observer.disconnect();
    });

    // 🌐 Expose functions globally for manual calls
    window.HeroAnimation.init = initializeAnimation;
    window.HeroAnimation.check = checkForHeroSection;

    console.log('🎯 Hero Animation script loaded and ready!');
})();