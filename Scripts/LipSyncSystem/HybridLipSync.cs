////using System.Collections;
////using UnityEngine;

////public class HybridLipSync : MonoBehaviour
////{
////    [Header("References")]
////    [SerializeField] private FacialExpressionController facialController;
////    [SerializeField] private AudioSource audioSource;

////    [Header("Settings")]
////    [SerializeField] private float amplitudeSensitivity = 100f;
////    [SerializeField] private float visemeChangeThreshold = 0.3f;
////    [SerializeField] private float visemeHoldTime = 0.1f;

////    [SerializeField] private AudioClip _clip;

////    private readonly BlendShapeWeight[][] _simpleVisemes = new[]
////    {
////        new[]
////        {
////            new BlendShapeWeight(BlendShape.MouthClose, 0.5f)
////        },

////        new[]
////        {
////            new BlendShapeWeight(BlendShape.JawOpen, 0.3f),
////            new BlendShapeWeight(BlendShape.MouthFunnel, 0.2f)
////        },

////        new[]
////        {
////            new BlendShapeWeight(BlendShape.JawOpen, 0.7f),
////            new BlendShapeWeight(BlendShape.MouthLowerDownLeft, 0.4f),
////            new BlendShapeWeight(BlendShape.MouthLowerDownRight, 0.4f)
////        },

////        new[]
////        {
////            new BlendShapeWeight(BlendShape.MouthSmileLeft, 0.5f),
////            new BlendShapeWeight(BlendShape.MouthSmileRight, 0.5f),
////            new BlendShapeWeight(BlendShape.JawOpen, 0.2f)
////        },

////        new[]
////        {
////            new BlendShapeWeight(BlendShape.MouthPucker, 0.6f),
////            new BlendShapeWeight(BlendShape.MouthFunnel, 0.4f)
////        }
////    };

////    private float[] _audioSamples = new float[256];
////    private int _currentVisemeIndex = 0;
////    private float _lastVisemeChangeTime;
////    private Coroutine _lipSyncCoroutine;

////    public void StartLipSync(AudioClip clip)
////    {
////        if (_lipSyncCoroutine != null) StopCoroutine(_lipSyncCoroutine);

////        audioSource.clip = clip;
////        audioSource.Play();

////        _lipSyncCoroutine = StartCoroutine(LipSyncRoutine());
////    }

////    private IEnumerator LipSyncRoutine()
////    {
////        while (audioSource.isPlaying)
////        {
////            float amplitude = GetAudioAmplitude();

////            if (Time.time - _lastVisemeChangeTime > visemeHoldTime)
////            {
////                UpdateViseme(amplitude);
////            }

////            yield return null;
////        }

////        facialController.ResetToNeutral(0.3f);
////    }

////    private float GetAudioAmplitude()
////    {
////        audioSource.GetOutputData(_audioSamples, 0);

////        float sum = 0;
////        foreach (float sample in _audioSamples)
////        {
////            sum += Mathf.Abs(sample);
////        }

////        return (sum / _audioSamples.Length) * amplitudeSensitivity;
////    }

////    private void UpdateViseme(float amplitude)
////    {
////        int targetViseme = 0;

////        if (amplitude < 0.1f) targetViseme = 0;
////        else if (amplitude < 0.3f) targetViseme = 1;
////        else if (amplitude < 0.6f) targetViseme = 2;
////        else targetViseme = Random.Range(3, _simpleVisemes.Length);

////        if (Mathf.Abs(targetViseme - _currentVisemeIndex) >= visemeChangeThreshold)
////        {
////            _currentVisemeIndex = targetViseme;
////            facialController.SetCustomExpression(_simpleVisemes[_currentVisemeIndex], 0.1f);
////            _lastVisemeChangeTime = Time.time;
////        }
////    }

////    public void StopLipSync()
////    {
////        if (_lipSyncCoroutine != null)
////        {
////            StopCoroutine(_lipSyncCoroutine);
////            _lipSyncCoroutine = null;
////        }

////        audioSource.Stop();
////        facialController.ResetToNeutral(0.3f);
////    }
////}


//using System.Collections;
//using UnityEngine;

//public class HybridLipSync : MonoBehaviour
//{
//    [Header("References")]
//    [SerializeField] private FacialExpressionController facialController;
//    [SerializeField] private AudioSource audioSource;

//    [Header("Settings")]
//    [SerializeField] private float amplitudeSensitivity = 10f;
//    [SerializeField] private float visemeChangeThreshold = 0.3f;
//    [SerializeField] private float visemeHoldTime = 0.1f;

//    private readonly BlendShapeWeight[][] _simpleVisemes = new[]
//    {
//        new[]
//        {
//            new BlendShapeWeight(BlendShape.MouthClose, 0.5f)
//        },

//        new[]
//        {
//            new BlendShapeWeight(BlendShape.JawOpen, 0.3f),
//            new BlendShapeWeight(BlendShape.MouthFunnel, 0.2f)
//        },

//        new[]
//        {
//            new BlendShapeWeight(BlendShape.JawOpen, 0.7f),
//            new BlendShapeWeight(BlendShape.MouthLowerDownLeft, 0.4f),
//            new BlendShapeWeight(BlendShape.MouthLowerDownRight, 0.4f)
//        },

//        new[]
//        {
//            new BlendShapeWeight(BlendShape.MouthSmileLeft, 0.5f),
//            new BlendShapeWeight(BlendShape.MouthSmileRight, 0.5f),
//            new BlendShapeWeight(BlendShape.JawOpen, 0.2f)
//        },

//        new[]
//        {
//            new BlendShapeWeight(BlendShape.MouthPucker, 0.6f),
//            new BlendShapeWeight(BlendShape.MouthFunnel, 0.4f)
//        }
//    };

//    private float[] _audioSamples = new float[256];
//    private int _currentVisemeIndex = 0;
//    private float _lastVisemeChangeTime;
//    private Coroutine _lipSyncCoroutine;

//    public bool IsPlaying { get; private set; }

//    public void StopLipSync()
//    {
//        if (_lipSyncCoroutine != null)
//        {
//            StopCoroutine(_lipSyncCoroutine);
//            _lipSyncCoroutine = null;
//        }

//        audioSource.Stop();
//        ResetToNeutral();
//        IsPlaying = false;
//    }

//    public void StartLipSync(AudioClip clip)
//    {
//        if (_lipSyncCoroutine != null) StopCoroutine(_lipSyncCoroutine);

//        audioSource.clip = clip;
//        audioSource.Play();
//        IsPlaying = true;

//        _lipSyncCoroutine = StartCoroutine(LipSyncRoutine());
//    }

//    private IEnumerator LipSyncRoutine()
//    {
//        while (audioSource.isPlaying)
//        {
//            float amplitude = GetAudioAmplitude();

//            if (Time.time - _lastVisemeChangeTime > visemeHoldTime)
//            {
//                UpdateViseme(amplitude);
//            }

//            yield return null;
//        }

//        ResetToNeutral();
//        IsPlaying = false;
//    }

//    private float GetAudioAmplitude()
//    {
//        audioSource.GetOutputData(_audioSamples, 0);

//        float sum = 0;
//        foreach (float sample in _audioSamples)
//        {
//            sum += Mathf.Abs(sample);
//        }

//        return (sum / _audioSamples.Length) * amplitudeSensitivity;
//    }

//    private void UpdateViseme(float amplitude)
//    {
//        int targetViseme = 0;

//        if (amplitude < 0.1f) targetViseme = 0;
//        else if (amplitude < 0.3f) targetViseme = 1;
//        else if (amplitude < 0.6f) targetViseme = 2;
//        else targetViseme = Random.Range(3, _simpleVisemes.Length);

//        if (Mathf.Abs(targetViseme - _currentVisemeIndex) >= visemeChangeThreshold)
//        {
//            _currentVisemeIndex = targetViseme;
//            facialController.SetCustomExpression(_simpleVisemes[_currentVisemeIndex], 0.1f);
//            _lastVisemeChangeTime = Time.time;
//        }
//    }

//    private void ResetToNeutral()
//    {
//        facialController.ResetToNeutral(0.3f);
//    }
//}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HybridLipSync : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FacialExpressionController _facialController;
    [SerializeField] private AudioSource _audioSource;

    [Header("Settings")]
    [SerializeField] private float _amplitudeSensitivity = 10f;
    //[SerializeField] private float _visemeChangeThreshold = 0.3f;
    [SerializeField] private float _visemeHoldTime = 0.12f;
    [SerializeField] private float _microMovementIntensity = 0.15f;

    [Header("Blink Settings")]
    [SerializeField] private float _minBlinkInterval = 2f;
    [SerializeField] private float _maxBlinkInterval = 4f;
    [SerializeField] private float _blinkDuration = 0.15f;
    [SerializeField] private float _doubleBinkChance = 0.3f;

    private struct VisemeCategory
    {
        public float MinAmplitude;
        public float MaxAmplitude;
        public BlendShapeWeight[][] Variants;
    }

    private VisemeCategory[] _visemeCategories;
    private float[] _audioSamples = new float[256];
    private int _currentCategoryIndex = 0;
    private int _currentVariantIndex = 0;
    private float _lastVisemeChangeTime;
    private float _nextBlinkTime;
    private bool _isBlinking;

    private Dictionary<int, BlendShapeWeight[]> _visemeCache = new Dictionary<int, BlendShapeWeight[]>();
    private System.Random _random = new System.Random();

    private Coroutine _lipSyncCoroutine;
    private Coroutine _blinkCoroutine;

    public bool IsPlaying { get; private set; }

    private void Awake()
    {
        InitializeVisemeCategories();
        ScheduleNextBlink();
    }

    private void InitializeVisemeCategories()
    {
        _visemeCategories = new[]
        {
            new VisemeCategory
            {
                MinAmplitude = 0f,
                MaxAmplitude = 0.1f,
                Variants = new[]
                {
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.MouthClose, 0.6f),
                        new BlendShapeWeight(BlendShape.MouthPressLeft, 0.2f),
                        new BlendShapeWeight(BlendShape.MouthPressRight, 0.2f)
                    },
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.MouthClose, 0.3f),
                        new BlendShapeWeight(BlendShape.JawOpen, 0.05f)
                    },
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.MouthClose, 0.1f),
                        new BlendShapeWeight(BlendShape.MouthDimpleLeft, 0.1f),
                        new BlendShapeWeight(BlendShape.MouthDimpleRight, 0.1f)
                    }
                }
            },

            new VisemeCategory
            {
                MinAmplitude = 0.1f,
                MaxAmplitude = 0.3f,
                Variants = new[]
                {
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.JawOpen, 0.15f),
                        new BlendShapeWeight(BlendShape.MouthFunnel, 0.2f),
                        new BlendShapeWeight(BlendShape.MouthClose, 0.1f)
                    },
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.JawOpen, 0.1f),
                        new BlendShapeWeight(BlendShape.NoseSneerLeft, 0.15f),
                        new BlendShapeWeight(BlendShape.NoseSneerRight, 0.15f),
                        new BlendShapeWeight(BlendShape.MouthClose, 0.3f)
                    },
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.MouthLowerDownLeft, 0.2f),
                        new BlendShapeWeight(BlendShape.MouthLowerDownRight, 0.2f),
                        new BlendShapeWeight(BlendShape.MouthUpperUpLeft, 0.25f),
                        new BlendShapeWeight(BlendShape.MouthUpperUpRight, 0.25f)
                    },
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.MouthSmileLeft, 0.2f),
                        new BlendShapeWeight(BlendShape.MouthSmileRight, 0.15f),
                        new BlendShapeWeight(BlendShape.JawOpen, 0.1f)
                    }
                }
            },

            new VisemeCategory
            {
                MinAmplitude = 0.3f,
                MaxAmplitude = 0.6f,
                Variants = new[]
                {
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.JawOpen, 0.5f),
                        new BlendShapeWeight(BlendShape.MouthLowerDownLeft, 0.4f),
                        new BlendShapeWeight(BlendShape.MouthLowerDownRight, 0.4f),
                        new BlendShapeWeight(BlendShape.MouthStretchLeft, 0.2f),
                        new BlendShapeWeight(BlendShape.MouthStretchRight, 0.2f)
                    },
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.JawOpen, 0.3f),
                        new BlendShapeWeight(BlendShape.MouthSmileLeft, 0.4f),
                        new BlendShapeWeight(BlendShape.MouthSmileRight, 0.4f),
                        new BlendShapeWeight(BlendShape.MouthDimpleLeft, 0.2f),
                        new BlendShapeWeight(BlendShape.MouthDimpleRight, 0.2f)
                    },
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.JawOpen, 0.35f),
                        new BlendShapeWeight(BlendShape.MouthPucker, 0.5f),
                        new BlendShapeWeight(BlendShape.MouthFunnel, 0.4f),
                        new BlendShapeWeight(BlendShape.MouthRollLower, 0.2f),
                        new BlendShapeWeight(BlendShape.MouthRollUpper, 0.2f)
                    },
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.JawOpen, 0.4f),
                        new BlendShapeWeight(BlendShape.MouthLeft, 0.2f),
                        new BlendShapeWeight(BlendShape.MouthLowerDownLeft, 0.3f),
                        new BlendShapeWeight(BlendShape.MouthSmileRight, 0.25f)
                    },
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.JawOpen, 0.25f),
                        new BlendShapeWeight(BlendShape.MouthDimpleLeft, 0.3f),
                        new BlendShapeWeight(BlendShape.MouthDimpleRight, 0.3f),
                        new BlendShapeWeight(BlendShape.MouthStretchLeft, 0.15f),
                        new BlendShapeWeight(BlendShape.MouthStretchRight, 0.15f)
                    }
                }
            },

            new VisemeCategory
            {
                MinAmplitude = 0.6f,
                MaxAmplitude = 1.0f,
                Variants = new[]
                {
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.JawOpen, 0.8f),
                        new BlendShapeWeight(BlendShape.MouthLowerDownLeft, 0.6f),
                        new BlendShapeWeight(BlendShape.MouthLowerDownRight, 0.6f),
                        new BlendShapeWeight(BlendShape.MouthStretchLeft, 0.4f),
                        new BlendShapeWeight(BlendShape.MouthStretchRight, 0.4f),
                        new BlendShapeWeight(BlendShape.BrowInnerUp, 0.15f)
                    },
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.JawOpen, 0.6f),
                        new BlendShapeWeight(BlendShape.MouthSmileLeft, 0.7f),
                        new BlendShapeWeight(BlendShape.MouthSmileRight, 0.7f),
                        new BlendShapeWeight(BlendShape.MouthUpperUpLeft, 0.3f),
                        new BlendShapeWeight(BlendShape.MouthUpperUpRight, 0.3f),
                        new BlendShapeWeight(BlendShape.CheekSquintLeft, 0.2f),
                        new BlendShapeWeight(BlendShape.CheekSquintRight, 0.2f)
                    },
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.JawOpen, 0.9f),
                        new BlendShapeWeight(BlendShape.MouthFunnel, 0.6f),
                        new BlendShapeWeight(BlendShape.MouthShrugUpper, 0.4f),
                        new BlendShapeWeight(BlendShape.MouthShrugLower, 0.4f),
                        new BlendShapeWeight(BlendShape.BrowOuterUpLeft, 0.3f),
                        new BlendShapeWeight(BlendShape.BrowOuterUpRight, 0.3f)
                    },
                    new[]
                    {
                        new BlendShapeWeight(BlendShape.JawOpen, 0.7f),
                        new BlendShapeWeight(BlendShape.MouthSmileLeft, 0.5f),
                        new BlendShapeWeight(BlendShape.MouthSmileRight, 0.65f),
                        new BlendShapeWeight(BlendShape.MouthRight, 0.2f),
                        new BlendShapeWeight(BlendShape.NoseSneerRight, 0.15f),
                        new BlendShapeWeight(BlendShape.BrowDownRight, 0.1f)
                    }
                }
            }
        };
    }

    public void StartLipSync(AudioClip clip)
    {
        StopLipSync();

        _audioSource.clip = clip;
        _audioSource.Play();
        IsPlaying = true;

        _lipSyncCoroutine = StartCoroutine(LipSyncRoutine());
        _blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    private IEnumerator LipSyncRoutine()
    {
        while (_audioSource.isPlaying)
        {
            float amplitude = GetAudioAmplitude();

            if (Time.time - _lastVisemeChangeTime > _visemeHoldTime)
            {
                UpdateViseme(amplitude);
                ApplyMicroMovements();
            }

            yield return null;
        }

        ResetToNeutral();
        IsPlaying = false;
    }

    private IEnumerator BlinkRoutine()
    {
        while (IsPlaying)
        {
            yield return new WaitForSeconds(_nextBlinkTime);

            if (!_isBlinking && Random.Range(0f, 1f) > 0.1f)
            {
                yield return PerformBlink();
            }

            ScheduleNextBlink();
        }
    }

    private IEnumerator PerformBlink()
    {
        _isBlinking = true;

        var blinkWeights = new[]
        {
            new BlendShapeWeight(BlendShape.EyeBlinkLeft, 1f),
            new BlendShapeWeight(BlendShape.EyeBlinkRight, 1f)
        };

        _facialController.SetCustomExpression(blinkWeights, _blinkDuration * 0.5f);

        yield return new WaitForSeconds(_blinkDuration);

        blinkWeights[0] = new BlendShapeWeight(BlendShape.EyeBlinkLeft, 0f);
        blinkWeights[1] = new BlendShapeWeight(BlendShape.EyeBlinkRight, 0f);

        _facialController.SetCustomExpression(blinkWeights, _blinkDuration * 0.5f);

        if (Random.Range(0f, 1f) < _doubleBinkChance)
        {
            yield return new WaitForSeconds(0.1f);
            yield return PerformBlink();
        }

        _isBlinking = false;
    }

    private void ScheduleNextBlink()
    {
        _nextBlinkTime = Random.Range(_minBlinkInterval, _maxBlinkInterval);

        if (IsPlaying && GetAudioAmplitude() > 0.6f)
        {
            _nextBlinkTime *= 0.7f;
        }
    }

    private float GetAudioAmplitude()
    {
        _audioSource.GetOutputData(_audioSamples, 0);

        float sum = 0;
        for (int i = 0; i < _audioSamples.Length; i++)
        {
            sum += Mathf.Abs(_audioSamples[i]);
        }

        return Mathf.Clamp01((sum / _audioSamples.Length) * _amplitudeSensitivity);
    }

    private void UpdateViseme(float amplitude)
    {
        int targetCategory = 0;

        for (int i = 0; i < _visemeCategories.Length; i++)
        {
            if (amplitude >= _visemeCategories[i].MinAmplitude &&
                amplitude <= _visemeCategories[i].MaxAmplitude)
            {
                targetCategory = i;
                break;
            }
        }

        if (targetCategory != _currentCategoryIndex ||
            Random.Range(0f, 1f) < 0.3f)
        {
            _currentCategoryIndex = targetCategory;
            var variants = _visemeCategories[targetCategory].Variants;
            _currentVariantIndex = _random.Next(variants.Length);

            var weights = GetOrCreateVisemeWeights(targetCategory, _currentVariantIndex);
            _facialController.SetCustomExpression(weights, _visemeHoldTime * 0.8f);
            _lastVisemeChangeTime = Time.time;
        }
    }

    private void ApplyMicroMovements()
    {
        if (Random.Range(0f, 1f) < 0.15f)
        {
            var microMovements = new List<BlendShapeWeight>();

            if (Random.Range(0f, 1f) < 0.3f)
            {
                float intensity = Random.Range(0.05f, _microMovementIntensity);

                if (Random.Range(0, 2) == 0)
                {
                    microMovements.Add(new BlendShapeWeight(BlendShape.BrowInnerUp, intensity));
                }
                else
                {
                    microMovements.Add(new BlendShapeWeight(BlendShape.BrowOuterUpLeft, intensity * 0.8f));
                    microMovements.Add(new BlendShapeWeight(BlendShape.BrowOuterUpRight, intensity));
                }
            }

            if (Random.Range(0f, 1f) < 0.2f)
            {
                float intensity = Random.Range(0.05f, _microMovementIntensity * 0.5f);
                microMovements.Add(new BlendShapeWeight(BlendShape.CheekPuff, intensity));
            }

            if (microMovements.Count > 0)
            {
                _facialController.SetCustomExpression(microMovements.ToArray(), 0.2f);
            }
        }
    }

    private BlendShapeWeight[] GetOrCreateVisemeWeights(int categoryIndex, int variantIndex)
    {
        int key = categoryIndex * 100 + variantIndex;

        if (!_visemeCache.ContainsKey(key))
        {
            var original = _visemeCategories[categoryIndex].Variants[variantIndex];
            var copy = new BlendShapeWeight[original.Length];

            for (int i = 0; i < original.Length; i++)
            {
                float randomFactor = Random.Range(0.9f, 1.1f);
                float weight = Mathf.Clamp01(original[i].Weight * randomFactor);
                copy[i] = new BlendShapeWeight(original[i].BlendShape, weight);
            }

            _visemeCache[key] = copy;
        }

        return _visemeCache[key];
    }

    public void StopLipSync()
    {
        if (_lipSyncCoroutine != null)
        {
            StopCoroutine(_lipSyncCoroutine);
            _lipSyncCoroutine = null;
        }

        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }

        if (_audioSource != null)
            _audioSource.Stop();

        ResetToNeutral();
        IsPlaying = false;
        _visemeCache.Clear();
    }

    private void ResetToNeutral()
    {
        _facialController.ResetToNeutral(0.3f);
        _currentCategoryIndex = 0;
        _currentVariantIndex = 0;
    }

    private void OnDestroy()
    {
        StopLipSync();
    }
}