using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace Echotools.UI.Nodes;

/// <summary>
/// Self-contained pagination bar with left/right arrow buttons, numbered page buttons,
/// and a page label. Matches FFXIV's native pagination style (crafting log).
///
/// Usage:
///   var bar = new PaginationBar(pos, width, onPageChanged: page => { ... });
///   bar.SetTotalItems(itemCount, pageSize);
///   // In OnUpdate:
///   bar.Update();
///   // Add all nodes:
///   foreach (var node in bar.Nodes) AddNode(node);
/// </summary>
public unsafe class PaginationBar
{
    private const int VisiblePageButtons = 5;
    private const float ArrowW = 36f;
    private const float ArrowH = 26f;
    private const float PageBtnW = 24f;
    private const float PageBtnH = 26f;
    private const float PageBtnGap = 2f;
    private const float ArrowGap = 4f;
    private const float LabelW = 100f;
    private const float LabelH = 20f;

    public readonly PaginationArrowButton PrevButton;
    public readonly PaginationArrowButton NextButton;
    public readonly PaginationLabel Label;
    public readonly PaginationPageButton[] PageButtons = new PaginationPageButton[VisiblePageButtons];

    private readonly float _areaX;
    private readonly float _areaW;
    private readonly float _y;
    private readonly Action<int>? _onPageChanged;

    private int _currentPage;
    private int _totalItems;
    private int _pageSize = 100;
    private int _pendingPageDelta;
    private int _pendingPageJump = -1;

    /// <summary>Current 0-based page index.</summary>
    public int CurrentPage => _currentPage;

    /// <summary>Total number of pages.</summary>
    public int TotalPages => Math.Max(1, (_totalItems + _pageSize - 1) / _pageSize);

    /// <summary>All nodes that need to be added to the addon via AddNode().</summary>
    public NodeBase[] Nodes { get; }

    /// <summary>
    /// Create a pagination bar.
    /// </summary>
    /// <param name="position">Top-left position of the pagination area.</param>
    /// <param name="width">Total available width for the bar.</param>
    /// <param name="onPageChanged">Callback when the page changes. Receives the new 0-based page index.</param>
    public PaginationBar(Vector2 position, float width, Action<int>? onPageChanged = null)
    {
        _areaX = position.X;
        _areaW = width;
        _y = position.Y;
        _onPageChanged = onPageChanged;

        PrevButton = new PaginationArrowButton(PaginationArrowDirection.Left)
        {
            Size = new Vector2(ArrowW, ArrowH),
            Position = new Vector2(0, _y),
        };
        PrevButton.OnClick = () => _pendingPageDelta = -1;

        for (var i = 0; i < VisiblePageButtons; i++)
        {
            var slotIdx = i;
            PageButtons[i] = new PaginationPageButton
            {
                Size = new Vector2(PageBtnW, PageBtnH),
                Position = new Vector2(0, _y),
                String = "",
                IsVisible = false,
            };
            PageButtons[i].OnClick = () => _pendingPageJump = PageButtons[slotIdx].PageIndex;
        }

        NextButton = new PaginationArrowButton(PaginationArrowDirection.Right)
        {
            Size = new Vector2(ArrowW, ArrowH),
            Position = new Vector2(0, _y),
        };
        NextButton.OnClick = () => _pendingPageDelta = 1;

        Label = new PaginationLabel
        {
            Position = new Vector2(_areaX + _areaW - LabelW, _y + 3),
            String = "",
        };

        // Build nodes array for easy AddNode iteration
        var nodes = new NodeBase[VisiblePageButtons + 3]; // prev + pages + next + label
        nodes[0] = PrevButton;
        for (var i = 0; i < VisiblePageButtons; i++)
            nodes[1 + i] = PageButtons[i];
        nodes[VisiblePageButtons + 1] = NextButton;
        nodes[VisiblePageButtons + 2] = Label;
        Nodes = nodes;
    }

    /// <summary>
    /// Set the total number of items and page size. Resets to page 0.
    /// </summary>
    public void SetTotalItems(int totalItems, int pageSize = 100)
    {
        _totalItems = totalItems;
        _pageSize = pageSize;
        _currentPage = 0;
    }

    /// <summary>
    /// Update the pagination state. Call this every frame from OnUpdate.
    /// Processes deferred page changes and updates button positions/states.
    /// </summary>
    public void Update()
    {
        var totalPages = TotalPages;

        // Process deferred page changes
        if (_pendingPageJump >= 0)
        {
            var newPage = Math.Clamp(_pendingPageJump, 0, totalPages - 1);
            _pendingPageJump = -1;
            if (newPage != _currentPage)
            {
                _currentPage = newPage;
                _onPageChanged?.Invoke(_currentPage);
            }
        }
        else if (_pendingPageDelta != 0)
        {
            var newPage = Math.Clamp(_currentPage + _pendingPageDelta, 0, totalPages - 1);
            _pendingPageDelta = 0;
            if (newPage != _currentPage)
            {
                _currentPage = newPage;
                _onPageChanged?.Invoke(_currentPage);
            }
        }

        // Enable/disable arrows
        PrevButton.IsEnabled = _currentPage > 0;
        NextButton.IsEnabled = _currentPage < totalPages - 1;

        // Calculate sliding window of page buttons (current page in the middle)
        var windowStart = Math.Max(0, _currentPage - 2);
        var windowEnd = Math.Min(totalPages, windowStart + VisiblePageButtons);
        windowStart = Math.Max(0, windowEnd - VisiblePageButtons);
        var visibleCount = windowEnd - windowStart;

        // Calculate total width and center horizontally
        var pageButtonsWidth = visibleCount > 0 ? visibleCount * PageBtnW + (visibleCount - 1) * PageBtnGap : 0;
        var totalWidth = ArrowW + ArrowGap + pageButtonsWidth + ArrowGap + ArrowW;
        var startX = _areaX + (_areaW - totalWidth) / 2;

        PrevButton.X = startX;

        var x = startX + ArrowW + ArrowGap;
        for (var i = 0; i < VisiblePageButtons; i++)
        {
            var pageIdx = windowStart + i;
            if (i < visibleCount)
            {
                PageButtons[i].IsVisible = true;
                PageButtons[i].X = x;
                PageButtons[i].PageIndex = pageIdx;
                PageButtons[i].String = (pageIdx + 1).ToString();
                PageButtons[i].IsCurrentPage = (pageIdx == _currentPage);
                x += PageBtnW + PageBtnGap;
            }
            else
            {
                PageButtons[i].IsVisible = false;
            }
        }

        NextButton.X = x + ArrowGap;

        // Update label
        if (_totalItems > 0)
        {
            var pageStart = _currentPage * _pageSize + 1;
            var pageEnd = Math.Min(pageStart + _pageSize - 1, _totalItems);
            Label.String = $"{pageStart}-{pageEnd}/{_totalItems}";
        }
        else
        {
            Label.String = "";
        }
    }
}
