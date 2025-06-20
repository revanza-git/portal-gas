/* ============================================
   Gas Monitoring Dashboard - Enhanced JavaScript
   ============================================ */

// Dashboard enhancement functions
$(document).ready(function() {
    
    // Initialize dashboard enhancements
    initializeDashboardEnhancements();
    
    // Add loading states for AJAX calls
    enhanceAjaxCalls();
    
    // Add real-time clock
    initializeClock();
    
    // Add tooltips and help text
    initializeTooltips();
    
    // Add keyboard shortcuts
    initializeKeyboardShortcuts();
    
    // Add auto-refresh functionality
    initializeAutoRefresh();
});

// Initialize dashboard enhancements
function initializeDashboardEnhancements() {
    
    // Add smooth animations to widgets
    $('.widget').each(function(index) {
        $(this).css('animation-delay', (index * 0.1) + 's')
               .addClass('fadeInUp animated');
    });
    
    // Add hover effects to charts
    $('#chartdiv, #chartdiv2, #chartdiv3, #chartdiv4').hover(
        function() {
            $(this).parent('.chart-container').addClass('chart-hover');
        },
        function() {
            $(this).parent('.chart-container').removeClass('chart-hover');
        }
    );
    
    // Add progress indicators to percentage values
    addProgressIndicators();
    
    // Initialize responsive chart resizing
    initializeResponsiveCharts();
}

// Add progress indicators
function addProgressIndicators() {
    const pasokanPercent = parseInt($('#prosentase_pasokan').val());
    const penjualanPercent = parseInt($('#prosentase_penjualan').val());
    
    // Add progress bars to widget headers
    const pasokanProgress = `
        <div class="progress-indicator" style="margin-top: 10px;">
            <div class="progress-bar" style="width: ${pasokanPercent}%; background: rgba(255,255,255,0.3); height: 4px; border-radius: 2px;"></div>
        </div>
    `;
    
    const penjualanProgress = `
        <div class="progress-indicator" style="margin-top: 10px;">
            <div class="progress-bar" style="width: ${penjualanPercent}%; background: rgba(255,255,255,0.3); height: 4px; border-radius: 2px;"></div>
        </div>
    `;
    
    $('.widget.bg-success').append(pasokanProgress);
    $('.widget.bg-warning').append(penjualanProgress);
}

// Enhance AJAX calls with loading states
function enhanceAjaxCalls() {
    
    // Override original updateDashboards function
    window.originalUpdateDashboards = window.updateDashboards;
    window.updateDashboards = function(tahun) {
        showLoadingState();
        
        // Call original function
        if (window.originalUpdateDashboards) {
            window.originalUpdateDashboards(tahun);
        }
        
        // Hide loading after delay
        setTimeout(hideLoadingState, 1500);
    };
    
    // Override original updateGraphs function
    window.originalUpdateGraphs = window.updateGraphs;
    window.updateGraphs = function(rd) {
        showLoadingState();
        
        // Call original function
        if (window.originalUpdateGraphs) {
            window.originalUpdateGraphs(rd);
        }
        
        // Hide loading after delay
        setTimeout(hideLoadingState, 2000);
    };
}

// Show loading state
function showLoadingState() {
    $('.loading-spinner').show();
    $('.widget').addClass('loading');
    $('.chart-container').addClass('loading');
    
    // Add loading overlay to charts
    $('.chart-container').each(function() {
        if (!$(this).find('.loading-overlay').length) {
            $(this).append('<div class="loading-overlay"><i class="fa fa-spinner fa-spin fa-2x"></i></div>');
        }
    });
}

// Hide loading state
function hideLoadingState() {
    $('.loading-spinner').hide();
    $('.widget').removeClass('loading');
    $('.chart-container').removeClass('loading');
    $('.loading-overlay').remove();
    
    // Re-add animations
    $('.widget').removeClass('fadeInUp animated').addClass('fadeInUp animated');
}

// Initialize real-time clock
function initializeClock() {
    function updateClock() {
        const now = new Date();
        const timeString = now.toLocaleTimeString('id-ID', {
            hour12: false,
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        });
        
        const dateString = now.toLocaleDateString('id-ID', {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
        
        // Add clock to dashboard header if not exists
        if (!$('.dashboard-clock').length) {
            $('.dashboard-subtitle').after(`
                <div class="dashboard-clock" style="margin-top: 10px; font-size: 1em; opacity: 0.8;">
                    <i class="fa fa-clock-o"></i> <span class="time">${timeString}</span> | 
                    <span class="date">${dateString}</span>
                </div>
            `);
        } else {
            $('.dashboard-clock .time').text(timeString);
            $('.dashboard-clock .date').text(dateString);
        }
    }
    
    updateClock();
    setInterval(updateClock, 1000);
}

// Initialize tooltips
function initializeTooltips() {
    
    // Add tooltips to widgets
    $('.widget.bg-success').attr('title', 'Gas supply performance metrics - Target vs Achievement');
    $('.widget.bg-warning').attr('title', 'Gas sales performance metrics - Target vs Achievement');
    $('.bog-widget').attr('title', 'Boil-off Gas monitoring - Real-time BOG rates and efficiency');
    
    // Add tooltips to chart containers
    $('.chart-container').each(function() {
        const title = $(this).find('.chart-title').text();
        $(this).attr('title', `Interactive chart: ${title}`);
    });
    
    // Initialize Bootstrap tooltips
    $('[title]').tooltip({
        placement: 'top',
        trigger: 'hover'
    });
}

// Initialize keyboard shortcuts
function initializeKeyboardShortcuts() {
    $(document).keydown(function(e) {
        // Ctrl + R: Refresh dashboard
        if (e.ctrlKey && e.keyCode === 82) {
            e.preventDefault();
            refreshDashboard();
        }
        
        // Ctrl + 1-4: Focus on specific charts
        if (e.ctrlKey && e.keyCode >= 49 && e.keyCode <= 52) {
            e.preventDefault();
            const chartNumber = e.keyCode - 48;
            $(`#chartdiv${chartNumber === 1 ? '' : chartNumber}`)[0].scrollIntoView({
                behavior: 'smooth'
            });
        }
    });
    
    // Add keyboard shortcut info
    if (!$('.keyboard-shortcuts').length) {
        $('body').append(`
            <div class="keyboard-shortcuts" style="position: fixed; bottom: 20px; right: 20px; background: rgba(0,0,0,0.8); color: white; padding: 10px; border-radius: 5px; font-size: 12px; z-index: 1000; display: none;">
                <strong>Keyboard Shortcuts:</strong><br>
                Ctrl + R: Refresh Dashboard<br>
                Ctrl + 1-4: Focus Charts
            </div>
        `);
    }
    
    // Show shortcuts on Ctrl key hold
    $(document).keydown(function(e) {
        if (e.ctrlKey) {
            $('.keyboard-shortcuts').fadeIn();
        }
    }).keyup(function(e) {
        if (!e.ctrlKey) {
            $('.keyboard-shortcuts').fadeOut();
        }
    });
}

// Initialize auto-refresh
function initializeAutoRefresh() {
    let autoRefreshInterval;
    
    // Add auto-refresh toggle
    if (!$('.auto-refresh-toggle').length) {
        $('.dashboard-header .col-md-4').prepend(`
            <div class="auto-refresh-toggle" style="margin-bottom: 10px;">
                <label style="color: white; font-size: 0.9em;">
                    <input type="checkbox" id="autoRefresh" style="margin-right: 5px;"> 
                    🔄 Auto Refresh (5min)
                </label>
            </div>
        `);
    }
    
    $('#autoRefresh').change(function() {
        if ($(this).is(':checked')) {
            autoRefreshInterval = setInterval(refreshDashboard, 5 * 60 * 1000); // 5 minutes
            showNotification('Auto refresh enabled (5 minutes)', 'success');
        } else {
            clearInterval(autoRefreshInterval);
            showNotification('Auto refresh disabled', 'info');
        }
    });
}

// Refresh dashboard
function refreshDashboard() {
    showNotification('Refreshing dashboard...', 'info');
    const currentYear = $('select[onchange*="updateDashboards"]').val();
    if (window.updateDashboards) {
        window.updateDashboards(currentYear);
    }
}

// Show notifications
function showNotification(message, type = 'info') {
    const notificationClass = `alert-${type === 'success' ? 'success' : type === 'error' ? 'danger' : 'info'}`;
    
    const notification = $(`
        <div class="alert ${notificationClass} notification" style="position: fixed; top: 20px; right: 20px; z-index: 1050; min-width: 300px;">
            <i class="fa fa-${type === 'success' ? 'check' : type === 'error' ? 'exclamation-triangle' : 'info'}"></i>
            ${message}
        </div>
    `);
    
    $('body').append(notification);
    
    setTimeout(() => {
        notification.fadeOut(() => notification.remove());
    }, 3000);
}

// Initialize responsive charts
function initializeResponsiveCharts() {
    $(window).resize(function() {
        // Trigger chart resize events
        setTimeout(() => {
            if (window.chart && window.chart.invalidateSize) {
                window.chart.invalidateSize();
            }
            if (window.chart2 && window.chart2.invalidateSize) {
                window.chart2.invalidateSize();
            }
            if (window.chart3 && window.chart3.invalidateSize) {
                window.chart3.invalidateSize();
            }
            if (window.chart4 && window.chart4.invalidateSize) {
                window.chart4.invalidateSize();
            }
        }, 100);
    });
}

// Add loading styles
$('<style>')
    .prop('type', 'text/css')
    .html(`
        .loading {
            opacity: 0.7;
            pointer-events: none;
        }
        
        .loading-overlay {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(255,255,255,0.8);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 10;
        }
        
        .chart-hover {
            transform: scale(1.02);
            box-shadow: 0 8px 30px rgba(0,0,0,0.2);
        }
        
        .fadeInUp {
            animation-name: fadeInUp;
            animation-duration: 0.6s;
            animation-fill-mode: both;
        }
        
        @keyframes fadeInUp {
            from {
                opacity: 0;
                transform: translate3d(0, 40px, 0);
            }
            to {
                opacity: 1;
                transform: translate3d(0, 0, 0);
            }
        }
        
        .notification {
            animation: slideInRight 0.3s ease;
        }
        
        @keyframes slideInRight {
            from {
                transform: translateX(100%);
            }
            to {
                transform: translateX(0);
            }
        }
    `)
    .appendTo('head'); 