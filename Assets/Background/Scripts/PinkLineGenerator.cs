// Draw a pink, continuous curve connecting all major elements ('checkpoints') on screen (e.g. character, text)
// Use multiple cubic Bezier curves with matching tangents and curvatures at endpoints for smoothness
// Add some randomness to checkpoint positions for different looking curves every time
// Use Gaussian Elimination for generating control points
// Use the Bernstein polynomial form for Beizer curve point calculation

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkLineGenerator : MonoBehaviour
    {
    [SerializeField] public Vector2 target_aspect;
    // Camera required to calculate corresponding world coordinates from screen-space
    [SerializeField] public Camera reference_camera;
    [SerializeField] public Transform[] checkpoint_objects;
    // Chance of any single checkpoint being actually used in curve generation
    [SerializeField]
    [Range(0, 1)]
    public float selection_probability = 1f;
    // The number of intermediate draw points for every 1 unit of curve length
    [SerializeField] public int draw_density = 10;
    [SerializeField] public float z_offset = 0f;
    // Amount of random x and y offset when determining checkpoint position, in world coordinates
    [SerializeField] public float checkpoint_inaccuracy = 0f;
    // Amount of y offset when generating checkpoints at each end of the screen
    [SerializeField] public float endpoint_offset = 0f;
    // Animation progress; 0 to 1 draws the line, 1 to 2 erases/retracts it
    [SerializeField]
    [Range(0, 2)]
    public float progress = 0f;
    // A 'button' to regrenerate a new line shape/path, accessible by animations
    public bool generate_new_line = false;

    private LineRenderer linerenderer;
    private float screen_world_width;
    private float screen_world_height;
    // Tracker variable for progress 'phase' change detection
    private float last_progress;

    private List<Vector3> draw_points;
    private List<Vector3> draw_points_reverse;
    private int draw_length;


    void Start()
        {
        linerenderer = GetComponent<LineRenderer>();

        // Calculate screen size in terms of world coordinates
        float aspect = target_aspect.x / target_aspect.y;
        screen_world_height = reference_camera.orthographicSize * 2;
        screen_world_width = screen_world_height * aspect;

        // Update progress tracker
        last_progress = progress;

        // Generate initial line shape
        GenerateLine();
        }

    // Implement the progressive line draw and retraction effect depending on progress
    // Instead of creating new position arrays every time progress changes,
    // simply change the number of points drawn by the LineRenderer via its positionCount property
    // Reverse point order for the retraction effect
    void Update()
        {
        // normal_progress is always between 0 and 1 inclusive
        // normal_progress when progress > 1 reverses direction,
        // starting from 1 and ending at 0 for the line retraction effect
        float normal_progress = progress <= 1 ? progress : 2 - progress;
        linerenderer.positionCount = (int)(draw_length * normal_progress);
        linerenderer.SetPositions(draw_points.ToArray());

        // If the new line generation 'button' bool is activated
        if (generate_new_line)
            {
            GenerateLine();
            generate_new_line = false;
            }

        // When progress changes 'phase' from 0-1 to 1-2 and vice versa
        // Do nothing when progress is exactly 2 (floor is also 2), as it is not a phase change
        if (Mathf.Floor(progress) != Mathf.Floor(last_progress) && progress != 2)
            {
            draw_points.Reverse();
            }

        // Update progress tracker
        last_progress = progress;
        }

    void GenerateLine()
        {
        // Generate points to draw
        List<Vector2> initial_checkpoints = GenerateCheckPoints();
        draw_points = GenerateBezierPoints(initial_checkpoints);
        draw_length = draw_points.Count;
        }

    // Generate checkpoint positions from input array
    // Generate each-end-of-screen checkpoints
    List<Vector2> GenerateCheckPoints()
        {
        List<Vector2> checkpoints = new List<Vector2>();

        foreach (Transform checkpoint_object in checkpoint_objects)
            {
            if (Random.Range(0f, 1f) <= selection_probability)
                {
                // Exact position
                Vector3 position = checkpoint_object.position;

                // Add random offset
                Vector2 checkpoint = new Vector2(position.x, position.y);
                checkpoint += new Vector2(
                    // Do not randomise x too much, as the horizontal order of checkpoints changing can look ugly
                    checkpoint_inaccuracy * Random.Range(-0.5f, 0.5f),
                    checkpoint_inaccuracy * Random.Range(-1f, 1f));

                checkpoints.Add(checkpoint);
                }
            }

        // Sort by ascending x-position, left-to-right
        checkpoints.Sort((point1, point2) => point1.x.CompareTo(point2.x));

        // Generate endpoints using existing leftmost and rightmost checkpoints
        // No need to do this if there are no checkpoints to begin with
        if (checkpoints.Count > 0)
            {
            Vector2 leftmost_checkpoint =
                new Vector2(-screen_world_width / 2,
                checkpoints[0].y + endpoint_offset * Random.Range(-1f, 1f));
            Vector2 rightmost_checkpoint =
                new Vector2(screen_world_width / 2,
                checkpoints[checkpoints.Count - 1].y + endpoint_offset * Random.Range(-1f, 1f));

            checkpoints.Insert(0, leftmost_checkpoint);
            checkpoints.Add(rightmost_checkpoint);
            }

        return checkpoints;
        }

    // A single cubic Bezier curve requires 4 control points, the first and last are checkpoints
    // Therefore, calculate the second and third non-checkpoint control points using a special "apex point" B
    // Using the control points derived from this B allows matching tangents and curvatures between curves
    // The theory behind this is well explained in section 5 of the UCLA Math 149 "Cubic Spline Curves" document
    // Process: Solve system of linear equations regarding B and checkpoint positions
    List<Vector3> GenerateBezierPoints(List<Vector2> checkpoints)
        {
        int n = checkpoints.Count - 1;
        List<Vector3> draw_points = new List<Vector3>();

        // M, the 1-4-1 coefficient matrix
        float[,] M = new float[n - 1, n - 1];

        for (int i = 1; i < n; i++)
            {
            if (i - 2 >= 0) { M[i - 1, i - 2] = 1f; }
            M[i - 1, i - 1] = 4f;
            if (i <= n - 2) { M[i - 1, i] = 1f; }
            }

        // C, the right hand side of the system of equations
        Vector2[] C = new Vector2[n - 1];

        for (int i = 1; i < n; i++)
            {
            Vector2 c = 6 * checkpoints[i];
            if (i == 1) { c -= checkpoints[0]; }
            if (i == n - 1) { c -= checkpoints[n]; }

            C[i - 1] = c;
            }

        // B, target
        Vector2[] B_partial = SolveLinearSystem(M, C);

        // Size of B equal to the number of checkpoints
        Vector2[] B = new Vector2[n + 1];

        for (int i = 1; i < n; i++)
            {
            B[i] = B_partial[i - 1];
            }

        // Add first and last checkpoints as B0 and Bn respectively
        B[0] = checkpoints[0];
        B[n] = checkpoints[n];

        // Generate intermediate control points using B and generate Bezier points
        for (int i = 0; i < n; i++)
            {
            // First control point
            Vector2 p0 = checkpoints[i];
            // Second and third control points
            Vector2 p1 = 2f / 3f * B[i] + 1f / 3f * B[i + 1];
            Vector2 p2 = 1f / 3f * B[i] + 2f / 3f * B[i + 1];
            // Last control point
            Vector2 p3 = checkpoints[i + 1];

            // iterations = distance * density, use density if distance is too small
            // Distance is a fast approximation
            int draw_iterations =
                (int)Mathf.Max(Vector2.Distance(p0, p1)
                + Vector2.Distance(p1, p2)
                + Vector2.Distance(p2, p3)
                * draw_density, draw_density);

            // Actually calculate curve points
            for (int draw_iteration = 0; draw_iteration < draw_iterations; draw_iteration++)
                {
                Vector2 bezier =
                    Bernstein(new Vector2[4] { p0, p1, p2, p3 },
                    (float)draw_iteration / (float)draw_iterations);

                draw_points.Add(new Vector3(bezier.x, bezier.y, z_offset));
                }
            }

        // Draw last checkpoint, since this is omitted from the above loop
        draw_points.Add(new Vector3(checkpoints[n].x, checkpoints[n].y, z_offset));

        return draw_points;
        }

    // Perform Gaussian elimination on an augmented matrix
    Vector2[] SolveLinearSystem(float[,] coeff_matrix, Vector2[] const_matrix)
        {
        int n = const_matrix.Length;
        Vector2[] solution = new Vector2[n];

        // Forward elimination 
        for (int i = 0; i < n - 1; i++)
            {
            for (int j = i + 1; j < n; j++)
                {
                float factor = coeff_matrix[j, i] / coeff_matrix[i, i];
                for (int k = i; k < n; k++)
                    {
                    coeff_matrix[j, k] -= factor * coeff_matrix[i, k];
                    }
                const_matrix[j] -= factor * const_matrix[i];
                }
            }

        // Calculate last solution
        solution[n - 1] = const_matrix[n - 1] / coeff_matrix[n - 1, n - 1];

        // Substitute up from here for the rest
        for (int i = n - 2; i >= 0; i--)
            {
            Vector2 rhs = const_matrix[i];
            for (int j = i + 1; j < n; j++)
                {
                rhs -= coeff_matrix[i, j] * solution[j];
                }
            solution[i] = rhs / coeff_matrix[i, i];
            }

        return solution;
        }

    // Calculate the Bezier curve draw point given 4 control points and a progress value t
    // 0 <= t <= 1
    Vector2 Bernstein(Vector2[] controlpoints, float t)
        {
        float t2 = Mathf.Pow(t, 2);
        float t3 = Mathf.Pow(t, 3);

        return
            controlpoints[0] * (-t3 + 3 * t2 - 3 * t + 1)
            + controlpoints[1] * (3 * t3 - 6 * t2 + 3 * t)
            + controlpoints[2] * (-3 * t3 + 3 * t2)
            + controlpoints[3] * t3;
        }
    }
