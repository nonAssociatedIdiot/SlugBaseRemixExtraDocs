﻿using UnityEngine;
using SlugBase.DataTypes;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using Menu;

namespace SlugBase.Features
{
    using static FeatureTypes;

    /// <summary>
    /// Built-in <see cref="Feature"/>s describing the player.
    /// </summary>
    public static class PlayerFeatures
    {
        // TODO: Test
        /// <summary>"color": Player body and UI color.</summary>
        public static readonly PlayerFeature<Color> SlugcatColor = PlyColor("color");

        // TODO: Test
        /// <summary>"body_color": Player body color, overriding "color".</summary>
        public static readonly PlayerFeature<PaletteColor> BodyColor = PlyPaletteColor("body_color");

        // TODO: Test
        /// <summary>"body_color_starved": Player body color when starving, overriding "color" and "body_color".</summary>
        public static readonly PlayerFeature<PaletteColor> BodyColorStarved = PlyPaletteColor("body_color_starved");

        // TODO: Test
        /// <summary>"eye_color": Player eye color.</summary>
        public static readonly PlayerFeature<PaletteColor> EyeColor = PlyPaletteColor("eye_color");

        // TODO: Test
        /// <summary>"auto_grab_batflies": Grab batflies on collision.</summary>
        public static readonly PlayerFeature<bool> AutoGrabFlies = PlyBool("auto_grab_batflies");

        // TODO: Test
        /// <summary>"weight": Weight multiplier.</summary>
        public static readonly PlayerFeature<float[]> WeightMul = PlyFloats("weight", 1, 2);

        // TODO: Test
        /// <summary>"tunnel_speed": Move speed in tunnels.</summary>
        public static readonly PlayerFeature<float[]> TunnelSpeedMul = PlyFloats("tunnel_speed", 1, 2);

        // TODO: Test
        /// <summary>"climb_speed": Move speed in tunnels.</summary>
        public static readonly PlayerFeature<float[]> ClimbSpeedMul = PlyFloats("climb_speed", 1, 2);

        // TODO: Test
        /// <summary>"walk_speed": Standing move speed.</summary>
        public static readonly PlayerFeature<float[]> WalkSpeedMul = PlyFloats("walk_speed", 1, 2);

        // TODO: Test
        /// <summary>"crouch_stealth": Standing move speed.</summary>
        public static readonly PlayerFeature<float[]> CrouchStealth = PlyFloats("crouch_stealth", 1, 2);

        // TODO: Test
        /// <summary>"throw_skill": Spear damage and speed.</summary>
        public static readonly PlayerFeature<int[]> ThrowSkill = PlyInts("throw_skill", 1, 2);

        // TODO: Test
        /// <summary>"lung_capacity": Time underwater before drowning.</summary>
        public static readonly PlayerFeature<float[]> LungsCapacityMul = PlyFloats("lung_capacity", 1, 2);

        // TODO: Test
        /// <summary>"loudness": Sound alert multiplier.</summary>
        public static readonly PlayerFeature<float[]> LoudnessMul = PlyFloats("loudness", 1, 2);
        
        // TODO: Test
        /// <summary>"alignments": Initial community reputation.</summary>
        public static readonly PlayerFeature<Dictionary<CreatureCommunities.CommunityID, RepOverride>> CommunityAlignments = new("alignments", json =>
        {
            var obj = json.AsObject();
            var reps = new Dictionary<CreatureCommunities.CommunityID, RepOverride>();
            foreach (var pair in obj)
            {
                reps[new CreatureCommunities.CommunityID(pair.Key)] = new(pair.Value);
            }
            return reps;
        });

        // TODO: Test base
        /// <summary>"diet": Edibility and nourishment of foods.</summary>
        public static readonly PlayerFeature<Diet> Diet = new("diet", json => new Diet(json));
    }

    /// <summary>
    /// Built-in <see cref="Feature"/>s describing the game.
    /// </summary>
    public static class GameFeatures
    {
        // TODO: Test
        /// <summary>"karma": Initial karma.</summary>
        public static readonly GameFeature<int> Karma = GameInt("karma");

        // TODO: Test
        /// <summary>"karma_cap": Initial karma cap.</summary>
        public static readonly GameFeature<int> KarmaCap = GameInt("karma_cap");

        // TODO: Test
        /// <summary>"den": Initial room, plus backups from highest to lowest priority.</summary>
        public static readonly GameFeature<string[]> Den = GameStrings("den", 1);

        // TODO: Test
        /// <summary>"guide_overseer": Player guide overseer color index.</summary>
        public static readonly GameFeature<int> GuideOverseer = GameInt("guide_overseer");

        // TODO: Test
        /// <summary>"has_dreams": Player guide overseer color index.</summary>
        public static readonly GameFeature<bool> HasDreams = GameBool("has_dreams");

        // TODO: Test
        /// <summary>"cycle_length_min": Minimum cycle length in minutes.</summary>
        public static readonly GameFeature<float> CycleLengthMin = GameFloat("cycle_length_min");

        // TODO: Test
        /// <summary>"cycle_length_max": Maximum cycle length in minutes.</summary>
        public static readonly GameFeature<float> CycleLengthMax = GameFloat("cycle_length_max");

        // TODO: Test
        /// <summary>"perma_unlock_gates": Maximum cycle length in minutes.</summary>
        public static readonly GameFeature<bool> PermaUnlockGates = GameBool("perma_unlock_gates");

        // TODO: Test
        /// <summary>"food_min": Food needed to sleep.</summary>
        public static readonly GameFeature<int> FoodMin = GameInt("food_min");

        // TODO: Test
        /// <summary>"food_max": Maximum food stored during a cycle.</summary>
        public static readonly GameFeature<int> FoodMax = GameInt("food_max");

        // TODO: Test
        /// <summary>The scene for this slugcat on the select menu.</summary>
        public static readonly GameFeature<MenuScene.SceneID> SelectMenuScene = GameExtEnum<MenuScene.SceneID>("select_menu_scene");

        // TODO: Test
        /// <summary>The scene for this slugcat on the select menu when ascended.</summary>
        public static readonly GameFeature<MenuScene.SceneID> SelectMenuSceneAscended = GameExtEnum<MenuScene.SceneID>("select_menu_scene_ascended");

        // TODO: Test
        /// <summary>The character to use for creature spawns and room connections.</summary>
        public static readonly GameFeature<SlugcatStats.Name> WorldState = GameExtEnum<SlugcatStats.Name>("world_state");
    }
}